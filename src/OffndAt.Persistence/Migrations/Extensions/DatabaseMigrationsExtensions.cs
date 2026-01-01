using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
namespace OffndAt.Persistence.Migrations.Extensions;


/// <summary>
///     Contains extension methods for configuring the database migrations.
/// </summary>
public static class DatabaseMigrationsExtensions
{
    /// <summary>
    ///     Ensures all migrations.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The configured application builder.</returns>
    public static IApplicationBuilder EnsureMigrations(this IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<DbContext>();
        context?.Database.Migrate();

        return builder;
    }
}
