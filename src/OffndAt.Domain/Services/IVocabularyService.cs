using OffndAt.Domain.Enumerations;

namespace OffndAt.Domain.Services;

/// <summary>
///     Defines the contract for vocabulary management operations.
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
