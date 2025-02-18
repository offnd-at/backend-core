namespace OffndAt.Contracts.Themes;

using System.ComponentModel;

/// <summary>
///     Represents the supported themes response.
/// </summary>
public sealed class GetSupportedThemesResponse
{
    /// <summary>
    ///     Gets the supported themes collection.
    /// </summary>
    [Description("The list of supported themes.")]
    public required IEnumerable<ThemeDto> Themes { get; init; }
}
