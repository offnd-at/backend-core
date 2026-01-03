using System.ComponentModel;

namespace OffndAt.Contracts.Links.Responses;

/// <summary>
///     Represents the generate link response.
/// </summary>
public sealed class GenerateLinkResponse
{
    /// <summary>
    ///     Gets the URL.
    /// </summary>
    [Description("The generated URL.")]
    public required string Url { get; init; }

    /// <summary>
    ///     Gets the stats URL.
    /// </summary>
    [Description("The pointer URL to stats for the generated link.")]
    public required string StatsUrl { get; init; }
}
