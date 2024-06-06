namespace OffndAt.Contracts.Languages;

/// <summary>
///     Represents the supported languages response.
/// </summary>
/// <param name="languages">The supported languages collection.</param>
public sealed class GetSupportedLanguagesResponse(IEnumerable<LanguageDto> languages)
{
    /// <summary>
    ///     Gets the supported languages collection.
    /// </summary>
    public IEnumerable<LanguageDto> Languages { get; } = languages;
}
