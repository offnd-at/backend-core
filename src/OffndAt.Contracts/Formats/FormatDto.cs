namespace OffndAt.Contracts.Formats;

using System.ComponentModel;

/// <summary>
///     Represents the format data transfer object.
/// </summary>
public sealed class FormatDto
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    [Description("The unique identifier of the format.")]
    public required int Value { get; init; }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    [Description("The name of the format.")]
    public required string Name { get; init; }
}
