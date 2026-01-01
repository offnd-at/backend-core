using MassTransit;
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
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Core.Constants;
using OffndAt.Infrastructure.Core.Data;
using OffndAt.Infrastructure.Core.Data.Settings;
using OffndAt.Infrastructure.Core.Http.Cors.Settings;
using OffndAt.Infrastructure.Core.Logging.Settings;
using OffndAt.Infrastructure.Core.Messaging;
using OffndAt.Infrastructure.Core.Messaging.Settings;
using OffndAt.Infrastructure.Core.Settings;
using OffndAt.Infrastructure.Phrases;
using OffndAt.Infrastructure.Urls;
using OffndAt.Infrastructure.Words;
using OffndAt.Persistence.Data;
using Polly;
using Polly.Retry;

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
        services.Configure<OpenObserveLoggerSettings>(configuration.GetSection(OpenObserveLoggerSettings.SettingsKey));
        services.Configure<GithubDataSourceSettings>(configuration.GetSection(GithubDataSourceSettings.SettingsKey));

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
        services.AddScoped<IFileLoader, GithubFileLoader>();
        services.AddScoped<IVocabularyLoader, GithubVocabularyLoader>();
        services.AddScoped<IGitHubClient>(serviceProvider =>
        {
            var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;
            return new GitHubClient(new ProductHeaderValue(applicationSettings.ApplicationName));
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

        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<CorsSettings>>().Value;

        services.AddCors(options => options.AddDefaultPolicy(builder => builder
            .SetIsOriginAllowed(origin =>
                settings.AllowedOrigins.Any(allowedOrigin => allowedOrigin.Equals(
                    new Uri(origin).Host,
                    StringComparison.OrdinalIgnoreCase)))
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

                cfg.QueryDelay = TimeSpan.FromSeconds(10);
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
                    });

                factoryConfigurator.UseMessageRetry(retryConfigurator => retryConfigurator.Interval(3, TimeSpan.FromSeconds(3)));

                factoryConfigurator.ConfigureEndpoints(context);
            });
        });

        services.TryAddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }
}
