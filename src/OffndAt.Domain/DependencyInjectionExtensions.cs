using Microsoft.Extensions.DependencyInjection;
using OffndAt.Domain.Services;

namespace OffndAt.Domain;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers domain services in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services) => services.AddDomainServices();

    /// <summary>
    ///     Registers domain service implementations in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IVocabularyService, VocabularyService>();

        return services;
    }
}
