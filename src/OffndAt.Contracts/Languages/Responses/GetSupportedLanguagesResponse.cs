using System.ComponentModel;
using OffndAt.Contracts.Languages.Dtos;

namespace OffndAt.Contracts.Languages.Responses;

/// <summary>
///     Represents the supported languages response.
/// </summary>
public sealed class GetSupportedLanguagesResponse
{
    /// <summary>
    ///     Gets the supported languages collection.
    /// </summary>
    [Description("The list of supported languages.")]
    public required IEnumerable<LanguageDto> Languages { get; init; }
}
