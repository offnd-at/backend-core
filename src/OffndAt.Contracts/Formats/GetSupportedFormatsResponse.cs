namespace OffndAt.Contracts.Formats;

/// <summary>
///     Represents the supported formats response.
/// </summary>
/// <param name="formats">The supported formats collection.</param>
public sealed class GetSupportedFormatsResponse(IEnumerable<FormatDto> formats)
{
    /// <summary>
    ///     Gets the supported formats collection.
    /// </summary>
    public IEnumerable<FormatDto> Formats { get; } = formats;
}
