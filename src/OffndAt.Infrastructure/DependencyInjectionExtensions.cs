using System.Net.Http.Headers;
using System.Reflection;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Octokit;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Application.Core.Abstractions.Phrases;
using OffndAt.Application.Core.Abstractions.Urls;
using OffndAt.Application.Core.Abstractions.Words;
using OffndAt.Application.Core.Exceptions;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Exceptions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Authentication.ApiKey;
using OffndAt.Infrastructure.Authentication.Settings;
using OffndAt.Infrastructure.Core.Constants;
using OffndAt.Infrastructure.Core.Data;
using OffndAt.Infrastructure.Core.Data.Settings;
using OffndAt.Infrastructure.Core.HealthChecks;
using OffndAt.Infrastructure.Core.Http.Cors.Settings;
using OffndAt.Infrastructure.Core.Messaging;
using OffndAt.Infrastructure.Core.Messaging.Settings;
using OffndAt.Infrastructure.Core.Settings;
using OffndAt.Infrastructure.Core.Telemetry.Settings;
using OffndAt.Infrastructure.Phrases;
using OffndAt.Infrastructure.Urls;
using OffndAt.Infrastructure.Words;
using OffndAt.Persistence.Data;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
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
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICaseConverter, CaseConverter>();
        services.AddScoped<IPhraseGenerator, PhraseGenerator>();
        services.AddScoped<IUrlMaker, UrlMaker>();
        services.AddScoped<IFileLoader, GitHubFileLoader>();
        services.AddScoped<IVocabularyLoader, GitHubVocabularyLoader>();
        services.AddScoped<IGitHubClient>(serviceProvider =>
        {
            var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;
            return new GitHubClient(new ProductHeaderValue(applicationSettings.AppName, applicationSettings.Version));
        });

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
    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services) =>
        services.AddResiliencePipeline<string, Result<Phrase>>(
            ResiliencePolicies.PhraseGeneratorRetryPolicyName,
            builder => builder.AddRetry(
                new RetryStrategyOptions<Result<Phrase>>
                {
                    Delay = TimeSpan.Zero,
                    BackoffType = DelayBackoffType.Constant,
                    MaxRetryAttempts = 10,
                    ShouldHandle = new PredicateBuilder<Result<Phrase>>().HandleResult(result =>
                        result.IsFailure && result.Error == DomainErrors.Phrase.AlreadyInUse)
                }));

    /// <summary>
    ///     Registers the MassTransit producer with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMassTransitProducer(this IServiceCollection services, IConfiguration configuration)
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

        services.TryAddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }

    /// <summary>
    ///     Registers the MassTransit consumer with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="assemblies">The assemblies containing message consumers, sagas, saga state machines, and activities.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMassTransitConsumer(
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

                factoryConfigurator.PrefetchCount = messageBrokerSettings.PrefetchCount;

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
    ///     Registers the Open Telemetry services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelemetrySettings>(configuration.GetSection(TelemetrySettings.SettingsKey));

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
                .AddMeter(InstrumentationOptions.MeterName)
                .AddOtlpExporter(exporterOptions => exporterOptions.Endpoint = new Uri(telemetrySettings.ExporterEndpoint)))
            .WithTracing(options => options
                // TODO: trace sampler
                .AddAspNetCoreInstrumentation(instrumentationOptions =>
                    instrumentationOptions.Filter = context => context.Request.Method != HttpMethod.Options.Method)
                .AddHttpClientInstrumentation()
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
}
