using System.ComponentModel;


namespace OffndAt.Contracts.Themes;/// <summary>
///     Response model containing available themes.
/// </summary>
public sealed class GetSupportedThemesResponse
{
    /// <summary>
    ///     Gets the supported themes collection.
    /// </summary>
    [Description("The list of supported themes.")]
    public required IEnumerable<ThemeDto> Themes { get; init; }
}
