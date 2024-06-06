namespace OffndAt.Contracts.Links;

/// <summary>
///     Represents the link data transfer object.
/// </summary>
public sealed class LinkDto(string targetUrl, int visits)
{
    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    public string TargetUrl { get; } = targetUrl;

    /// <summary>
    ///     Gets the visits count.
    /// </summary>
    public int Visits { get; } = visits;
}
