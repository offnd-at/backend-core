using Microsoft.EntityFrameworkCore;
using OffndAt.Persistence.Data;

namespace OffndAt.Services.MigrationRunner;

/// <summary>
///     Represents the background service that runs database migrations.
/// </summary>
/// <param name="hostApplicationLifetime">The host application lifetime.</param>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The logger.</param>
internal sealed class MigrationRunnerBackgroundService(
    IHostApplicationLifetime hostApplicationLifetime,
    IServiceProvider serviceProvider,
    ILogger<MigrationRunnerBackgroundService> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<OffndAtDbContext>();

                logger.LogInformation("Starting database migration...");

                await dbContext.Database.MigrateAsync(stoppingToken);

                logger.LogInformation("Database migration completed successfully");

                break;
            }
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }
}
