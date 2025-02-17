namespace OffndAt.Contracts.Formats;

using System.ComponentModel;

/// <summary>
///     Represents the supported formats response.
/// </summary>
public sealed class GetSupportedFormatsResponse
{
    /// <summary>
    ///     Gets the supported formats collection.
    /// </summary>
    [Description("The list of supported formats.")]
    public required IEnumerable<FormatDto> Formats { get; init; }
}
