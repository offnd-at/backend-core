using System.ComponentModel;
namespace OffndAt.Contracts.Languages;


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
