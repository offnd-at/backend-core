namespace OffndAt.Application.Links.ReadModels;

/// <summary>
///     Represents a read model for a link visit log entry.
/// </summary>
public sealed class LinkVisitLogEntryReadModel
{
    /// <summary>
    ///     Gets or sets the unique identifier for the log entry.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    ///     Gets or sets the link identifier.
    /// </summary>
    public required Guid LinkId { get; init; }

    /// <summary>
    ///     Gets or sets the date and time when the link was visited, expressed in UTC.
    /// </summary>
    public required DateTimeOffset VisitedAt { get; init; }

    /// <summary>
    ///     Gets or sets the IP address of the visitor.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    ///     Gets or sets the user agent string of the visitor's client.
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    ///     Gets or sets the referrer.
    /// </summary>
    public string? Referrer { get; init; }
}
