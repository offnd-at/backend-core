using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OffndAt.Application.Abstractions.Links;

namespace OffndAt.Infrastructure.Links;

/// <summary>
///     Background worker responsible for periodically flushing accumulated link visit counts
///     from the <see cref="LinkVisitTracker" /> into the persistent statistics storage.
/// </summary>
/// <param name="serviceProvider">The service provider.</param>
/// <param name="configuration">The configuration.</param>
/// <param name="logger">The logger.</param>
internal sealed class LinkVisitFlushWorker(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<LinkVisitFlushWorker> logger) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var flushInterval = configuration.GetValue("LinkVisitCacheFlushInterval", TimeSpan.FromMinutes(1));
        using var timer = new PeriodicTimer(flushInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await FlushAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to flush link visit statistics");
            }
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await FlushAsync(stoppingToken);
        await base.StopAsync(stoppingToken);
    }

    /// <summary>
    ///     Flushes accumulated visit counts from the visit tracker into the persistent statistics repository.
    /// </summary>
    /// <param name="cancellationToken"></param>
    private async Task FlushAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var visitTracker = scope.ServiceProvider.GetRequiredService<ILinkVisitTracker>();
        var summaryRepository = scope.ServiceProvider.GetRequiredService<ILinkVisitSummaryRepository>();

        var batch = await visitTracker.DrainAsync(cancellationToken);
        if (batch.Count == 0)
        {
            return;
        }

        await summaryRepository.UpsertTotalVisitsForManyAsync(
            batch.Select(item => (item.LinkId, item.VisitCount)).ToList(),
            cancellationToken);

        logger.LogInformation("Successfully flushed {ItemCount} visit tracker cache items into the persistent store", batch.Count);
    }
}
