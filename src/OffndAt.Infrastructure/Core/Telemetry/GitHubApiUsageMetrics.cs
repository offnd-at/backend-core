using System.Diagnostics.Metrics;
using Microsoft.Extensions.Options;
using OffndAt.Infrastructure.Core.Telemetry.Settings;

namespace OffndAt.Infrastructure.Core.Telemetry;

/// <summary>
///     Provides metrics for GitHub API usage.
/// </summary>
internal sealed class GitHubApiUsageMetrics
{
    private readonly Gauge<int> _apiRateLimitRemaining;
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestLatency;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GitHubApiUsageMetrics" /> class.
    /// </summary>
    /// <param name="meterFactory">The meter factory.</param>
    /// <param name="telemetryOptions">The telemetry options.</param>
    public GitHubApiUsageMetrics(IMeterFactory meterFactory, IOptions<TelemetrySettings> telemetryOptions)
    {
        var meter = meterFactory.Create(telemetryOptions.Value.MeterName);

        _requestCounter = meter.CreateCounter<long>(
            "github.api.requests",
            "{request}",
            "The number of requests made to the GitHub API");

        _requestLatency = meter.CreateHistogram<double>(
            "github.api.request_duration",
            "ms",
            "The duration of requests made to the GitHub API");

        _apiRateLimitRemaining = meter.CreateGauge<int>(
            "github.api.rate_limit.remaining",
            "{request}",
            "The remaining GitHub API rate limit quota");
    }

    /// <summary>
    ///     Records a GitHub API request.
    /// </summary>
    public void RecordRequest() => _requestCounter.Add(1);

    /// <summary>
    ///     Records the request latency.
    /// </summary>
    /// <param name="durationMs">The duration in milliseconds.</param>
    public void RecordRequestLatency(double durationMs) => _requestLatency.Record(durationMs);

    /// <summary>
    ///     Records the API rate limit remaining count.
    /// </summary>
    /// <param name="remaining">The remaining rate limit count.</param>
    public void RecordRemainingRateLimit(int remaining) => _apiRateLimitRemaining.Record(remaining);
}
