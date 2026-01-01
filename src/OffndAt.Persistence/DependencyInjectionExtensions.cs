using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Domain.Repositories;
using OffndAt.Persistence.Core.Cache.Settings;
using OffndAt.Persistence.Data;
using OffndAt.Persistence.Repositories;
using OffndAt.Persistence.Settings;

namespace OffndAt.Persistence;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the persistence services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddPersistenceSettings(configuration)
            .AddInMemoryCache(configuration)
            .AddDatabaseContext()
            .AddPersistenceServices()
            .AddMemoryCache();

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

    /// <summary>
    ///     Registers the persistence settings with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddPersistenceSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceSettings>(configuration.GetSection(PersistenceSettings.SettingsKey));

        return services;
    }

    /// <summary>
    ///     Registers the database context with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<PersistenceSettings>>().Value;

        services.AddDbContext<OffndAtDbContext>(options =>
            options.UseNpgsql(
                settings.ConnectionString,
                innerOptions => innerOptions.MigrationsAssembly("OffndAt.Persistence")));

        return services;
    }

    /// <summary>
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<DbContext, OffndAtDbContext>();
        services.AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<OffndAtDbContext>());
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<OffndAtDbContext>());
        services.AddScoped<ILinksRepository, LinksRepository>();
        services.AddScoped<IVocabulariesRepository, VocabulariesRepository>();

        return services;
    }
}
