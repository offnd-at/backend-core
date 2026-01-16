namespace OffndAt.Application.Links.ReadModels;

/// <summary>
///     Represents a read model for a link visit summary.
/// </summary>
public sealed class LinkVisitSummaryReadModel
{
    /// <summary>
    ///     Gets or sets the link identifier.
    /// </summary>
    public required Guid LinkId { get; init; }

    /// <summary>
    ///     Gets or sets the total visits count.
    /// </summary>
    public required long TotalVisits { get; init; }
}
