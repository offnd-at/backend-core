﻿namespace OffndAt.Services.Api.Endpoints.Extensions;

using Examples;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
///     Contains extension methods for OpenAPI examples configuration.
/// </summary>
internal static class OpenApiExamplesExtensions
{
    /// <summary>
    ///     Registers the examples with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddOpenApiExamplesFromAssemblyContaining<TAssembly>(this IServiceCollection services)
    {
        var assembly = typeof(TAssembly).Assembly;

        var examplesDescriptors = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false })
            .SelectMany(
                type => type
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IOpenApiExample<>))
                    .Select(interfaceType => ServiceDescriptor.Singleton(interfaceType, type)))
            .ToArray();

        services.TryAddEnumerable(examplesDescriptors);

        return services;
    }
}
