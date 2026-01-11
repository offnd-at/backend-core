using System.ComponentModel;

namespace OffndAt.Contracts.Links.Dtos;

/// <summary>
///     Represents the link data transfer object.
/// </summary>
public sealed class LinkDto
{
    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    [Description("The target URL to which the link will redirect.")]
    public required string TargetUrl { get; init; }

    /// <summary>
    ///     Gets the visits count.
    /// </summary>
    [Description("The number of times the link has been visited.")]
    public required int Visits { get; init; }

    /// <summary>
    ///     Gets or sets the created at date and time in UTC format.
    /// </summary>
    [Description("The date and time when the link was created, in UTC.")]
    public required DateTimeOffset CreatedAt { get; init; }
}
