using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Abstractions.Telemetry;

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

    /// <summary>
    ///     Records a link visit/redirection.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    void RecordLinkVisit(Language language, Theme theme);

    /// <summary>
    ///     Records a cache hit during redirection.
    /// </summary>
    void RecordRedirectCacheHit();

    /// <summary>
    ///     Records a cache miss during redirection.
    /// </summary>
    void RecordRedirectCacheMiss();
}
