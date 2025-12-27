namespace OffndAt.Persistence;

using Application.Core.Abstractions.Data;
using Core.Cache.Settings;
using Data;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repositories;
using Settings;

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
            .AddDatabaseContext(configuration)
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
        _ = services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SettingsKey));
        _ = services.AddMemoryCache();

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
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        var persistenceSettings = configuration.GetSection(PersistenceSettings.SettingsKey).Get<PersistenceSettings>()
            ?? throw new InvalidOperationException($"Missing configuration section: {PersistenceSettings.SettingsKey}");

        services.AddDbContext<OffndAtDbContext>(
            options =>
                options.UseNpgsql(
                    persistenceSettings.ConnectionString,
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
