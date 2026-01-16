using System.ComponentModel;

namespace OffndAt.Contracts.Links.Dtos;

/// <summary>
///     Represents a single visit for a link.
/// </summary>
public sealed class LinkVisitDto
{
    /// <summary>
    ///     Gets or sets the date and time when the link was visited, expressed in UTC.
    /// </summary>
    [Description("The date and time when the link was visited, expressed in UTC.")]
    public required DateTimeOffset VisitedAt { get; init; }

    /// <summary>
    ///     Gets or sets the IP address of the visitor.
    /// </summary>
    [Description("The IPv4 or IPv6 address of the visitor. May be null if not captured or anonymized.")]
    public string? IpAddress { get; init; }

    /// <summary>
    ///     Gets or sets the user agent string of the visitor's client.
    /// </summary>
    [Description(
        "The user agent string identifying the visitor's browser, device, and operating system. " +
        "May be null if not provided by the client.")]
    public string? UserAgent { get; init; }

    /// <summary>
    ///     Gets or sets the referrer.
    /// </summary>
    [Description(
        "The URL of the referring page from which the visitor navigated to this link. " +
        "May be null if the visitor accessed the link directly or the referrer was not sent.")]
    public string? Referrer { get; init; }
}
