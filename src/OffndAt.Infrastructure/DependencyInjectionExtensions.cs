namespace OffndAt.Infrastructure;

using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Application.Core.Abstractions.Phrases;
using Application.Core.Abstractions.Urls;
using Application.Core.Abstractions.Words;
using Core.Constants;
using Core.Data;
using Core.Data.Settings;
using Core.Http.Cors.Settings;
using Core.Messaging;
using Core.Messaging.Settings;
using Core.Settings;
using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Octokit;
using Persistence.Data;
using Phrases;
using Polly;
using Polly.Retry;
using Urls;
using Words;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers infrastructure settings in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<ApplicationSettings>(configuration.GetSection(ApplicationSettings.SettingsKey));
        _ = services.Configure<GithubDataSourceSettings>(configuration.GetSection(GithubDataSourceSettings.SettingsKey));

        return services;
    }

    /// <summary>
    ///     Registers infrastructure service implementations in the dependency injection container.
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
        services.AddScoped<IGitHubClient>(
            serviceProvider =>
            {
                var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new GitHubClient(new ProductHeaderValue(applicationSettings.ApplicationName));
            });

        return services;
    }

    /// <summary>
    ///     Registers CORS policies in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(CorsSettings.SettingsKey).Get<CorsSettings>()
            ?? throw new InvalidOperationException($"Missing configuration section: {CorsSettings.SettingsKey}");

        _ = services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SettingsKey));

        _ = services.AddCors(
            options => options.AddDefaultPolicy(
                builder => builder
                    .SetIsOriginAllowed(
                        origin => corsSettings.AllowedOrigins.Any(
                            allowedOrigin => allowedOrigin.Equals(new Uri(origin).Host, StringComparison.OrdinalIgnoreCase)))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));

        return services;
    }

    /// <summary>
    ///     Registers resilience policies in the dependency injection container.
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
                    ShouldHandle = new PredicateBuilder<Result<Phrase>>().HandleResult(
                        result => result.IsFailure && result.Error == DomainErrors.Phrase.AlreadyInUse)
                }));

    /// <summary>
    ///     Registers MassTransit message producer in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMassTransitProducer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        services.AddMassTransit(
            configurator =>
            {
                configurator.AddEntityFrameworkOutbox<OffndAtDbContext>(
                    cfg =>
                    {
                        cfg.UsePostgres();
                        cfg.UseBusOutbox();

                        cfg.QueryDelay = TimeSpan.FromSeconds(10);
                    });

                configurator.SetKebabCaseEndpointNameFormatter();

                configurator.UsingRabbitMq(
                    (context, factoryConfigurator) =>
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
