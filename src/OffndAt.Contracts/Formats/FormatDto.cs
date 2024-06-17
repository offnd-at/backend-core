namespace OffndAt.Contracts.Formats;

/// <summary>
///     Represents the format data transfer object.
/// </summary>
public sealed class FormatDto(int value, string name)
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
