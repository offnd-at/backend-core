namespace OffndAt.Contracts.Links;

/// <summary>
///     Represents the generate link response.
/// </summary>
/// <param name="url">The URL.</param>
/// <param name="statsUrl">The stats URL.</param>
public sealed class GenerateLinkResponse(string url, string statsUrl)
{
    /// <summary>
    ///     Gets the URL.
    /// </summary>
    public string Url { get; } = url;

    /// <summary>
    ///     Gets the stats URL.
    /// </summary>
    public string StatsUrl { get; } = statsUrl;
}
