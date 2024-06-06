namespace OffndAt.Contracts.Links;

/// <summary>
///     Represents the generate link request.
/// </summary>
public sealed class GenerateLinkRequest
{
    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    public string TargetUrl { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the language identifier.
    /// </summary>
    public int LanguageId { get; init; }

    /// <summary>
    ///     Gets the theme identifier.
    /// </summary>
    public int ThemeId { get; init; }

    /// <summary>
    ///     Gets the format identifier.
    /// </summary>
    public int FormatId { get; init; }
}
