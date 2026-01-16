using Microsoft.EntityFrameworkCore;
using Npgsql;
using OffndAt.Persistence.Data;

namespace OffndAt.Services.MigrationRunner.Jobs;

/// <summary>
///     Represents the background service that runs database migrations.
/// </summary>
/// <param name="hostApplicationLifetime">The host application lifetime.</param>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="logger">The logger.</param>
internal sealed class MigrationRunner(
    IHostApplicationLifetime hostApplicationLifetime,
    IServiceProvider serviceProvider,
    ILogger<MigrationRunner> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int maxRetries = 10;
        var retryDelay = TimeSpan.FromSeconds(5);
        var retryCount = 0;

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<OffndAtDbContext>();

                    logger.LogInformation("Starting database migration...");

                    await dbContext.Database.MigrateAsync(stoppingToken);

                    logger.LogInformation("Database migration completed successfully");

                    break;
                }
                catch (NpgsqlException ex) when (retryCount < maxRetries)
                {
                    retryCount++;

                    logger.LogWarning(
                        ex,
                        "Migration attempt {RetryCount} failed: {Message}. Retrying in {RetryDelay}s...",
                        retryCount,
                        ex.Message,
                        retryDelay.TotalSeconds);

                    await Task.Delay(retryDelay, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Migration failed after {RetryCount} attempts", retryCount);
                    throw;
                }
            }
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }
}
