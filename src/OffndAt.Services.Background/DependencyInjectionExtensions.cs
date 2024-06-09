namespace OffndAt.Services.Background;

using Microsoft.Extensions.DependencyInjection;
using Tasks;

public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the background services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddBackground(this IServiceCollection services) =>
        services
            .AddMediator()
            .AddHostedServices();

    /// <summary>
    ///     Registers the MediatR with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        return services;
    }

    /// <summary>
    ///     Registers the hosted services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<IntegrationEventConsumerBackgroundService>();

        return services;
    }
}
