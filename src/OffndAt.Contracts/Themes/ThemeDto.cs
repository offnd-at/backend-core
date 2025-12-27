using System.ComponentModel;


namespace OffndAt.Contracts.Themes;/// <summary>
///     Data transfer object for theme information in API responses.
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
