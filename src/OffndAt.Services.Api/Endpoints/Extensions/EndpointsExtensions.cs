namespace OffndAt.Services.Api.Endpoints.Extensions;

using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
///     Contains extension methods for endpoints configuration.
/// </summary>
internal static class EndpointsExtensions
{
    /// <summary>
    ///     Registers the endpoints with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddEndpointsFromAssemblyContaining<TAssembly>(this IServiceCollection services)
    {
        var assembly = typeof(TAssembly).Assembly;

        var serviceDescriptors = assembly.DefinedTypes
            .Where(
                type => type is { IsAbstract: false, IsInterface: false } &&
                        type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    /// <summary>
    ///     Maps the endpoints for all available versions, based on the folder hierarchy.
    /// </summary>
    /// <param name="app">The web application builder.</param>
    /// <returns>The configured web application builder.</returns>
    public static IApplicationBuilder MapEndpointsForAllVersions(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        var groupedByVersion = endpoints.GroupBy(
            endpoint =>
            {
                var type = endpoint.GetType();
                var namespaceParts = type.Namespace?.Split('.') ?? [];
                var versionPart = namespaceParts.FirstOrDefault(part => part.StartsWith("V", StringComparison.OrdinalIgnoreCase));
                return int.TryParse(versionPart?.TrimStart('V', 'v'), out var version) ? version : 1;
            });

        foreach (var group in groupedByVersion)
        {
            var version = group.Key;

            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(version))
                .Build();

            var versionedGroup = app
                .MapGroup("v{version:apiVersion}")
                .WithApiVersionSet(apiVersionSet);

            foreach (var endpoint in group)
            {
                endpoint.MapEndpoint(versionedGroup);
            }
        }

        return app;
    }
}
