using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OffndAt.Persistence.Migrations.Extensions;

/// <summary>
///     Contains extension methods for configuring the database migrations.
/// </summary>
public static class DatabaseMigrationsExtensions
{
    /// <summary>
    ///     Ensures all migrations. Does nothing in the Production environment.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The configured application builder.</returns>
    public static IApplicationBuilder EnsureMigrationsIfDevelopment(this IApplicationBuilder builder)
    {
        var env = builder.ApplicationServices.GetRequiredService<IHostEnvironment>();
        if (env.IsProduction())
        {
            return builder;
        }

        using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<DbContext>();
        context?.Database.Migrate();

        return builder;
    }
}
