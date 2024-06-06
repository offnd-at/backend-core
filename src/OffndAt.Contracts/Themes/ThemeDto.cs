namespace OffndAt.Contracts.Themes;

/// <summary>
///     Represents the theme data transfer object.
/// </summary>
public sealed class ThemeDto(int value, string name)
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    public int Value { get; } = value;

    /// <summary>
    ///     Gets the name.
    /// </summary>
    public string Name { get; } = name;
}
