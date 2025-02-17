namespace OffndAt.Services.EventsWorker;

using Infrastructure.Core.Messaging.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistence.Data;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the background services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddEventsWorker(this IServiceCollection services, IConfiguration configuration) =>
        services.AddMassTransitConsumer(configuration);

    /// <summary>
    ///     Registers the MassTransit consumer with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddMassTransitConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));

        _ = services.AddMassTransit(
            configurator =>
            {
                configurator.AddEntityFrameworkOutbox<OffndAtDbContext>(
                    cfg =>
                    {
                        _ = cfg.UsePostgres();

                        cfg.DuplicateDetectionWindow = TimeSpan.FromSeconds(60);
                    });

                configurator.SetKebabCaseEndpointNameFormatter();
                configurator.SetInMemorySagaRepositoryProvider();

                var assembly = typeof(Program).Assembly;

                configurator.AddConsumers(assembly);
                configurator.AddSagaStateMachines(assembly);
                configurator.AddSagas(assembly);
                configurator.AddActivities(assembly);

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

                        factoryConfigurator.UseMessageRetry(
                            retryConfigurator => retryConfigurator.Intervals(
                                500,
                                1000,
                                2000,
                                5000,
                                5000));

                        factoryConfigurator.ConfigureEndpoints(context);
                    });
            });

        return services;
    }
}
