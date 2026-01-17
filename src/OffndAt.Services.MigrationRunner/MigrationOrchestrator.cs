using Npgsql;

namespace OffndAt.Services.MigrationRunner;

/// <summary>
///     Orchestrates database migration execution with retry logic and exponential backoff.
/// </summary>
internal static class MigrationOrchestrator
{
    /// <summary>
    ///     Runs database migrations with retry logic for transient failures.
    ///     Uses exponential backoff strategy starting at 2 seconds, doubling on each retry, with a maximum delay of 30 seconds.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="maxRetries">The maximum number of retry attempts for transient failures.</param>
    /// <param name="initialRetryDelay">The initial delay before the first retry attempt.</param>
    /// <returns>A task representing the asynchronous migration operation.</returns>
    internal static async Task RunMigrationsWithRetryAsync(
        IServiceProvider services,
        ILogger logger,
        int maxRetries,
        TimeSpan initialRetryDelay)
    {
        var retryCount = 0;
        var retryDelay = initialRetryDelay;

        while (true)
        {
            try
            {
                await MigrationApplier.ApplyMigrationsAsync(services, logger);
                return;
            }
            catch (NpgsqlException ex) when (retryCount < maxRetries)
            {
                retryCount++;

                logger.LogWarning(
                    ex,
                    "Migration attempt {RetryCount}/{MaxRetries} failed: {Message}. Retrying in {RetryDelaySeconds}s...",
                    retryCount,
                    maxRetries,
                    ex.Message,
                    retryDelay.TotalSeconds);

                await Task.Delay(retryDelay);

                retryDelay = TimeSpan.FromSeconds(Math.Min(retryDelay.TotalSeconds * 2, 30));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration failed after {RetryCount} attempts with non-retryable error", retryCount);
                throw;
            }
        }
    }
}
