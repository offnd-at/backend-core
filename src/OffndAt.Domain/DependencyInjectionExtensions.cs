﻿namespace OffndAt.Domain;

using Microsoft.Extensions.DependencyInjection;
using Services;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the domain services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services) => services.AddDomainServices();

    /// <summary>
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IVocabularyService, VocabularyService>();

        return services;
    }
}
