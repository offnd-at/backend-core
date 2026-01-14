using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Domain.Repositories;
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
    ///     Registers the persistence settings with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddPersistenceSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceSettings>(configuration.GetSection(PersistenceSettings.SettingsKey));

        return services;
    }

    /// <summary>
    ///     Registers the database context with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        services.AddDbContext<OffndAtDbContext>((serviceProvider, options) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<PersistenceSettings>>().Value;

            options.UseNpgsql(
                settings.ConnectionString,
                innerOptions => innerOptions.MigrationsAssembly("OffndAt.Persistence"));
        });

        return services;
    }

    /// <summary>
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddScoped<DbContext, OffndAtDbContext>();
        services.AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<OffndAtDbContext>());
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<OffndAtDbContext>());
        services.AddScoped<ILinkRepository, LinkRepository>();

        return services;
    }
}
