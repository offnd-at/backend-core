using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Core.Abstractions.Telemetry;

/// <summary>
///     Provides metrics for link operations.
/// </summary>
public interface ILinkMetrics
{
    /// <summary>
    ///     Records a link creation.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    void RecordLinkCreation(Language language, Theme theme);
}
