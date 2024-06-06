namespace OffndAt.Contracts.Themes;

/// <summary>
///     Represents the supported themes response.
/// </summary>
/// <param name="themes">The supported themes collection.</param>
public sealed class GetSupportedThemesResponse(IEnumerable<ThemeDto> themes)
{
    /// <summary>
    ///     Gets the supported themes collection.
    /// </summary>
    public IEnumerable<ThemeDto> Themes { get; } = themes;
}
