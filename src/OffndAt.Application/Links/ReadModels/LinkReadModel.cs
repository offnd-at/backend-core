namespace OffndAt.Application.Links.ReadModels;

/// <summary>
///     Represents a read model for a link.
/// </summary>
public sealed class LinkReadModel
{
    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets or sets the phrase.
    /// </summary>
    public required string Phrase { get; init; }

    /// <summary>
    ///     Gets or sets the target URL.
    /// </summary>
    public required string TargetUrl { get; init; }

    /// <summary>
    ///     Gets or sets the language identifier.
    /// </summary>
    public required int LanguageId { get; init; }

    /// <summary>
    ///     Gets or sets the theme identifier.
    /// </summary>
    public required int ThemeId { get; init; }

    /// <summary>
    ///     Gets or sets the visit summary.
    /// </summary>
    public required LinkVisitSummaryReadModel VisitSummary { get; init; }

    /// <summary>
    ///     Gets or sets a collection of the most recent visit log entries (up to 10).
    /// </summary>
    public required IReadOnlyCollection<LinkVisitLogEntryReadModel> RecentEntries { get; init; }

    /// <summary>
    ///     Gets or sets the date and time when the link was created, expressed in UTC.
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
