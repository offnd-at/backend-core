using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Abstractions.Telemetry;

/// <summary>
///     Provides metrics for link visits.
/// </summary>
public interface IVisitMetrics
{
    /// <summary>
    ///     Records a link visit/redirection.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    void RecordVisit(Language language, Theme theme);
}
