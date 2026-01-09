namespace OffndAt.Infrastructure.Core.Abstractions.Telemetry;

/// <summary>
///     Provides metrics for GitHub API usage.
/// </summary>
public interface IGitHubApiUsageMetrics
{
    /// <summary>
    ///     Records a GitHub API request.
    /// </summary>
    void RecordRequest();

    /// <summary>
    ///     Records the request latency.
    /// </summary>
    /// <param name="durationMs">The duration in milliseconds.</param>
    void RecordRequestLatency(double durationMs);

    /// <summary>
    ///     Records the API rate limit remaining count.
    /// </summary>
    /// <param name="remaining">The remaining rate limit count.</param>
    void RecordRemainingRateLimit(int remaining);
}
