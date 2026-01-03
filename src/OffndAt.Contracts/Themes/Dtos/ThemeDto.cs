using System.ComponentModel;

namespace OffndAt.Contracts.Themes.Dtos;

/// <summary>
///     Represents the theme data transfer object.
/// </summary>
public sealed class ThemeDto
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    [Description("The unique identifier of the theme.")]
    public required int Value { get; init; }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    [Description("The name of the theme.")]
    public required string Name { get; init; }
}
