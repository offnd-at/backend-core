namespace OffndAt.Domain.Services;

using Enumerations;

/// <summary>
///     Represents the vocabulary service interface.
/// </summary>
public interface IVocabularyService
{
    /// <summary>
    ///     Generates grammatical properties for a noun vocabulary.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    /// <returns>A tuple containing randomly chosen grammatical number and grammatical gender.</returns>
    (GrammaticalNumber number, GrammaticalGender gender) GenerateGrammaticalPropertiesForNounVocabulary(Language language, Theme theme);
}
