using System.ComponentModel;
using OffndAt.Contracts.Themes.Dtos;

namespace OffndAt.Contracts.Themes.Responses;

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
