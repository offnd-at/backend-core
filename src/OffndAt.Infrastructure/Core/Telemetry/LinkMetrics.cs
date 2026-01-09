using System.Diagnostics.Metrics;
using Microsoft.Extensions.Options;
using OffndAt.Application.Core.Abstractions.Telemetry;
using OffndAt.Domain.Enumerations;
using OffndAt.Infrastructure.Core.Telemetry.Settings;

namespace OffndAt.Infrastructure.Core.Telemetry;

/// <summary>
///     Provides metrics for link operations.
/// </summary>
internal sealed class LinkMetrics : ILinkMetrics
{
    private readonly Counter<long> _linkCreationCounter;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkMetrics" /> class.
    /// </summary>
    /// <param name="meterFactory">The meter factory.</param>
    /// <param name="telemetryOptions">The telemetry options.</param>
    public LinkMetrics(IMeterFactory meterFactory, IOptions<TelemetrySettings> telemetryOptions)
    {
        var meter = meterFactory.Create(telemetryOptions.Value.MeterName);

        _linkCreationCounter = meter.CreateCounter<long>(
            "links.creations",
            "{link}",
            "The number of links created over time");
    }

    /// <inheritdoc />
    public void RecordLinkCreation(Language language, Theme theme) =>
        _linkCreationCounter.Add(
            1,
            new KeyValuePair<string, object?>("language", language.Code),
            new KeyValuePair<string, object?>("theme", theme.Name));
}
