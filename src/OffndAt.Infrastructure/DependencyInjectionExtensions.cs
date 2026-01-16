using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.RateLimiting;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Octokit;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Abstractions.Phrases;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Application.Abstractions.Urls;
using OffndAt.Application.Abstractions.Words;
using OffndAt.Application.Core.Constants;
using OffndAt.Application.Core.Errors;
using OffndAt.Application.Core.Exceptions;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Exceptions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Abstractions.Telemetry;
using OffndAt.Infrastructure.Authentication.ApiKey;
using OffndAt.Infrastructure.Authentication.Settings;
using OffndAt.Infrastructure.Core.Cache.Settings;
using OffndAt.Infrastructure.Core.Constants;
using OffndAt.Infrastructure.Core.HealthChecks;
using OffndAt.Infrastructure.Core.Http.Cors.Settings;
using OffndAt.Infrastructure.Core.Messaging;
using OffndAt.Infrastructure.Core.Messaging.Settings;
using OffndAt.Infrastructure.Core.Settings;
using OffndAt.Infrastructure.Core.Telemetry;
using OffndAt.Infrastructure.Core.Telemetry.Settings;
using OffndAt.Infrastructure.Data;
using OffndAt.Infrastructure.Data.Settings;
using OffndAt.Infrastructure.Links;
using OffndAt.Infrastructure.Phrases;
using OffndAt.Infrastructure.Repositories;
using OffndAt.Infrastructure.Urls;
using OffndAt.Infrastructure.Words;
using OffndAt.Persistence.Data;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace OffndAt.Infrastructure;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the infrastructure settings with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationSettings>(configuration.GetSection(ApplicationSettings.SettingsKey));
        services.Configure<GitHubDataSourceSettings>(configuration.GetSection(GitHubDataSourceSettings.SettingsKey));

        return services;
    }

    /// <summary>
    ///     Configures the forwarded headers options to correctly process client IP addresses and protocol headers when running behind a reverse
    ///     proxy.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddForwardedHeadersSettings(this IServiceCollection services)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }

    /// <summary>
    ///     Registers the rate limiting services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(
                    clientId,
                    _ =>
                        new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 120,
                            Window = TimeSpan.FromMinutes(1),
                            AutoReplenishment = true,
                            SegmentsPerWindow = 6,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
            });

            options.AddPolicy(
                RateLimitingPolicyNames.RedirectByPhrase,
                context =>
                {
                    var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetTokenBucketLimiter(
                        clientId,
                        _ =>
                            new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 60,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                                TokensPerPeriod = 10,
                                AutoReplenishment = true
                            });
                });

            options.OnRejected = async (context, _) =>
            {
                var problemDetailsService = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString(CultureInfo.InvariantCulture);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                await problemDetailsService.TryWriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails = new ProblemDetails
                        {
                            Title = "An application error occurred.",
                            Status = StatusCodes.Status429TooManyRequests,
                            Extensions = new Dictionary<string, object?>
                            {
                                {
                                    "errors", new[]
                                    {
                                        ApplicationErrors.TooManyRequests
                                    }
                                }
                            }
                        }
                    });
            };
        });

        return services;
    }

    /// <summary>
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="environment">The web host environment.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddSingleton<ILinkVisitTracker, LinkVisitTracker>();

        services.AddScoped<ICaseConverter, CaseConverter>();
        services.AddScoped<IPhraseGenerator, PhraseGenerator>();
        services.AddScoped<IUrlMaker, UrlMaker>();
        services.AddScoped<IFileLoader, GitHubFileLoader>();
        services.AddScoped<IVocabularyLoader, GitHubVocabularyLoader>();
        services.AddScoped<IVocabularyRepository, VocabularyRepository>();
        services.AddScoped<ILinkCache, LinkCache>();
        services.AddScoped<IGitHubClient>(serviceProvider =>
        {
            var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;
            return new GitHubClient(new ProductHeaderValue(applicationSettings.AppName, applicationSettings.Version));
        });

        if (!environment.IsEnvironment(EnvironmentNames.Testing))
        {
            services.AddHostedService<LinkVisitFlushWorker>();
        }

        return services;
    }

    /// <summary>
    ///     Registers the CORS policy with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SettingsKey));

        var settings = configuration.GetSection(CorsSettings.SettingsKey).Get<CorsSettings>() ??
            throw new InvalidOperationException($"Missing configuration section - {CorsSettings.SettingsKey}.");

        services.AddCors(options => options.AddDefaultPolicy(builder => builder
            .WithOrigins(settings.AllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        return services;
    }

    /// <summary>
    ///     Registers the resilience policies with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
    {
        services.AddResiliencePipeline<string, Result<Phrase>>(
            ResiliencePolicies.PhraseAlreadyInUsePolicyName,
            builder => builder.AddRetry(
                new RetryStrategyOptions<Result<Phrase>>
                {
                    Delay = TimeSpan.Zero,
                    BackoffType = DelayBackoffType.Constant,
                    UseJitter = false,
                    MaxRetryAttempts = 10,
                    ShouldHandle = new PredicateBuilder<Result<Phrase>>().HandleResult(result =>
                        result.IsFailure && result.Error == DomainErrors.Phrase.AlreadyInUse)
                }));

        services.AddResiliencePipeline<string>(
            ResiliencePolicies.GitHubApiPolicyName,
            builder =>
            {
                builder
                    .AddTimeout(TimeSpan.FromSeconds(15))
                    .AddRetry(
                        new RetryStrategyOptions
                        {
                            Delay = TimeSpan.FromSeconds(1),
                            BackoffType = DelayBackoffType.Exponential,
                            UseJitter = true,
                            MaxRetryAttempts = 3,
                            ShouldHandle = new PredicateBuilder()
                                .Handle<Exception>(ex => ex.InnerException is ApiException)
                                .Handle<ApiException>(ex =>
                                    ex.StatusCode is HttpStatusCode.TooManyRequests or >= HttpStatusCode.InternalServerError)
                                .Handle<RateLimitExceededException>()
                                .Handle<SecondaryRateLimitExceededException>()
                        })
                    .AddCircuitBreaker(
                        new CircuitBreakerStrategyOptions
                        {
                            FailureRatio = 0.25,
                            SamplingDuration = TimeSpan.FromSeconds(30),
                            MinimumThroughput = 10,
                            BreakDuration = TimeSpan.FromSeconds(10),
                            ShouldHandle = new PredicateBuilder()
                                .Handle<Exception>(ex => ex.InnerException is ApiException)
                                .Handle<ApiException>(ex => ex.StatusCode >= HttpStatusCode.InternalServerError)
                        });
            });

        return services;
    }

    /// <summary>
    ///     Registers the MassTransit for a producer role with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMassTransitForProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        services.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<OffndAtDbContext>(cfg =>
            {
                cfg.UsePostgres();
                cfg.UseBusOutbox();
                cfg.QueryDelay = TimeSpan.FromSeconds(1);
            });

            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.UsingRabbitMq((context, factoryConfigurator) =>
            {
                var messageBrokerSettings = context.GetRequiredService<IOptions<MessageBrokerSettings>>().Value;

                factoryConfigurator.Host(
                    messageBrokerSettings.Hostname,
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(messageBrokerSettings.Username);
                        hostConfigurator.Password(messageBrokerSettings.Password);
                        hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
                        hostConfigurator.PublisherConfirmation = true;
                    });

                // Since the API only produces messages, does not consume them
                factoryConfigurator.PrefetchCount = 0;
            });
        });

        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }

    /// <summary>
    ///     Registers the MassTransit for a consumer role with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="assemblies">The assemblies containing message consumers, sagas, saga state machines, and activities.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMassTransitForConsumer(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly[]? assemblies = null)
    {
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        services.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<OffndAtDbContext>(cfg =>
            {
                cfg.UsePostgres();
                cfg.UseBusOutbox();
                cfg.DuplicateDetectionWindow = TimeSpan.FromSeconds(60);
            });

            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumers(assemblies ?? [Assembly.GetCallingAssembly()]);

            configurator.UsingRabbitMq((context, factoryConfigurator) =>
            {
                var messageBrokerSettings = context.GetRequiredService<IOptions<MessageBrokerSettings>>().Value;

                factoryConfigurator.Host(
                    messageBrokerSettings.Hostname,
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(messageBrokerSettings.Username);
                        hostConfigurator.Password(messageBrokerSettings.Password);
                        hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
                    });

                if (messageBrokerSettings.PrefetchCount is null)
                {
                    throw new InvalidOperationException("Prefetch count is required for consumer workers");
                }

                factoryConfigurator.PrefetchCount = messageBrokerSettings.PrefetchCount.Value;

                factoryConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Exponential(
                        5,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(2));

                    retryConfigurator.Ignore<ValidationException>();
                    retryConfigurator.Ignore<DomainException>();
                });

                factoryConfigurator.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromSeconds(30);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(1);
                });

                factoryConfigurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    ///     Registers the MassTransit for both producer and consumer roles with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="assemblies">The assemblies containing message consumers, sagas, saga state machines, and activities.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMassTransitForProducerAndConsumer(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly[]? assemblies = null)
    {
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        services.AddMassTransit(configurator =>
        {
            configurator.AddEntityFrameworkOutbox<OffndAtDbContext>(cfg =>
            {
                cfg.UsePostgres();
                cfg.UseBusOutbox();
                cfg.DuplicateDetectionWindow = TimeSpan.FromSeconds(60);
                cfg.QueryDelay = TimeSpan.FromSeconds(1);
            });

            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumers(assemblies ?? [Assembly.GetCallingAssembly()]);

            configurator.UsingRabbitMq((context, factoryConfigurator) =>
            {
                var messageBrokerSettings = context.GetRequiredService<IOptions<MessageBrokerSettings>>().Value;

                factoryConfigurator.Host(
                    messageBrokerSettings.Hostname,
                    hostConfigurator =>
                    {
                        hostConfigurator.Username(messageBrokerSettings.Username);
                        hostConfigurator.Password(messageBrokerSettings.Password);
                        hostConfigurator.RequestedConnectionTimeout(TimeSpan.FromSeconds(30));
                        hostConfigurator.PublisherConfirmation = true;
                    });

                if (messageBrokerSettings.PrefetchCount is null)
                {
                    throw new InvalidOperationException("Prefetch count is required for consumer workers");
                }

                factoryConfigurator.PrefetchCount = messageBrokerSettings.PrefetchCount.Value;

                factoryConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Exponential(
                        5,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(2));

                    retryConfigurator.Ignore<ValidationException>();
                    retryConfigurator.Ignore<DomainException>();
                });

                factoryConfigurator.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromSeconds(30);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(1);
                });

                factoryConfigurator.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }

    /// <summary>
    ///     Registers the Open Telemetry services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelemetrySettings>(configuration.GetSection(TelemetrySettings.SettingsKey));

        services.AddSingleton<IGitHubApiUsageMetrics, GitHubApiUsageMetrics>();
        services.AddSingleton<ILinkMetrics, LinkMetrics>();

        var telemetrySettings = configuration.GetSection(TelemetrySettings.SettingsKey).Get<TelemetrySettings>() ??
            throw new InvalidOperationException($"Missing configuration section - {TelemetrySettings.SettingsKey}.");

        if (!telemetrySettings.Enabled)
        {
            return services;
        }

        var applicationSettings = configuration.GetSection(ApplicationSettings.SettingsKey).Get<ApplicationSettings>() ??
            throw new InvalidOperationException($"Missing configuration section - {ApplicationSettings.SettingsKey}.");

        services.AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService(applicationSettings.AppName))
            .WithMetrics(options => options
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddMeter("Polly")
                .AddMeter(InstrumentationOptions.MeterName)
                .AddMeter(OffndAtInstrumentationOptions.MeterName)
                .AddOtlpExporter(exporterOptions => exporterOptions.Endpoint = new Uri(telemetrySettings.ExporterEndpoint)))
            .WithTracing(options => options
                .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(telemetrySettings.SampleRate ?? 1.0)))
                .AddAspNetCoreInstrumentation(instrumentationOptions =>
                    instrumentationOptions.Filter = context => context.Request.Method != HttpMethod.Options.Method)
                .AddHttpClientInstrumentation()
                .AddSource("Polly")
                .AddSource(DiagnosticHeaders.DefaultListenerName)
                .AddEntityFrameworkCoreInstrumentation(instrumentationOptions =>
                {
                    instrumentationOptions.Filter = (_, command) =>
                    {
                        var shouldFilterOut = command.CommandText.Contains("FROM \"OutboxState\"") ||
                            command.CommandText.Contains("FROM \"OutboxMessage\"") ||
                            command.CommandText.Contains("FROM \"InboxState\"");

                        return !shouldFilterOut;
                    };
                    instrumentationOptions.SetDbStatementForText = true;
                    instrumentationOptions.SetDbStatementForStoredProcedure = true;
                })
                .AddOtlpExporter(exporterOptions => exporterOptions.Endpoint = new Uri(telemetrySettings.ExporterEndpoint)));

        return services;
    }

    /// <summary>
    ///     Registers the authentication services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SettingsKey));

        services.AddAuthorization();
        services.AddAuthentication(options => options.DefaultScheme = ApiKeyDefaults.AuthenticationScheme)
            .AddScheme<ApiKeyOptions, ApiKeyHandler>(ApiKeyDefaults.AuthenticationScheme, null);

        return services;
    }

    /// <summary>
    ///     Registers the health-checking services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddHealthMonitoring(this IServiceCollection services)
    {
        services.AddHttpClient<GitHubApiHealthCheck>((serviceProvider, client) =>
        {
            var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(applicationSettings.AppName, applicationSettings.Version));
        });

        services.AddHealthChecks()
            .AddDbContextCheck<OffndAtDbContext>("ef-core-db-context")
            .AddCheck<GitHubApiHealthCheck>("github-api");

        return services;
    }

    /// <summary>
    ///     Registers the memory cache with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInMemoryCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SettingsKey));
        services.AddMemoryCache();

        return services;
    }
}
