using System.Diagnostics.Metrics;
using OffndAt.Application.Core.Abstractions.Telemetry;
using OffndAt.Domain.Enumerations;
using OffndAt.Infrastructure.Core.Telemetry.Settings;

namespace OffndAt.Infrastructure.Core.Telemetry;

/// <summary>
///     Provides metrics for link visits.
/// </summary>
internal sealed class VisitMetrics : IVisitMetrics
{
    private readonly Counter<long> _visitCounter;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VisitMetrics" /> class.
    /// </summary>
    /// <param name="meterFactory">The meter factory.</param>
    public VisitMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(OffndAtInstrumentationOptions.MeterName);

        _visitCounter = meter.CreateCounter<long>(
            "offndat.links.visits",
            "{visit}",
            "The number of link visits and redirections");
    }

    /// <inheritdoc />
    public void RecordVisit(Language language, Theme theme) =>
        _visitCounter.Add(
            1,
            new KeyValuePair<string, object?>("language", language.Code),
            new KeyValuePair<string, object?>("theme", theme.Name));
}
