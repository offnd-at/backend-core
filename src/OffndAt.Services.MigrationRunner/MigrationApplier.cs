using Microsoft.EntityFrameworkCore;
using OffndAt.Persistence.Data;

namespace OffndAt.Services.MigrationRunner;

/// <summary>
///     Applies migrations to the database.
/// </summary>
internal static class MigrationApplier
{
    /// <summary>
    ///     Applies pending database migrations using Entity Framework Core.
    ///     Checks for pending migrations, logs their names, and applies them to the database.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="logger">The logger.</param>
    /// <returns>A task representing the asynchronous migration operation.</returns>
    internal static async Task ApplyMigrationsAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OffndAtDbContext>();

        logger.LogInformation("Checking for pending migrations...");

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        var pendingMigrationsList = pendingMigrations.ToList();
        if (pendingMigrationsList.Count == 0)
        {
            logger.LogInformation("No pending migrations found. Database is up to date");
            return;
        }

        logger.LogInformation(
            "Found {PendingMigrationsCount} pending migration(s): {PendingMigrations}",
            pendingMigrationsList.Count,
            string.Join(", ", pendingMigrationsList));

        logger.LogInformation("Applying migrations...");

        await dbContext.Database.MigrateAsync();

        logger.LogInformation("Successfully applied {MigrationsCount} migration(s)", pendingMigrationsList.Count);
    }
}
