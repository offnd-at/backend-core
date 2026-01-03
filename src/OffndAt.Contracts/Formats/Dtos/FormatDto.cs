using System.ComponentModel;

namespace OffndAt.Contracts.Formats.Dtos;

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
