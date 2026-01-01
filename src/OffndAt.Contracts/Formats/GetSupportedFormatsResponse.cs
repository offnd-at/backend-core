using System.ComponentModel;
namespace OffndAt.Contracts.Formats;


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
