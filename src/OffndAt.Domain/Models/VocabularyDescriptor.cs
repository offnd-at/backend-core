namespace OffndAt.Domain.Models;

using Enumerations;

/// <summary>
///     Describes vocabulary metadata including language, theme, and format.
/// </summary>
/// <param name="Language">The language.</param>
/// <param name="Theme">The theme.</param>
/// <param name="Offensiveness">The offensiveness.</param>
/// <param name="GrammaticalNumber">The grammatical number.</param>
/// <param name="GrammaticalGender">The grammatical gender.</param>
/// <param name="PartOfSpeech">The part of speech.</param>
public record VocabularyDescriptor(
    Language Language,
    Theme Theme,
    Offensiveness Offensiveness,
    GrammaticalNumber GrammaticalNumber,
    GrammaticalGender GrammaticalGender,
    PartOfSpeech PartOfSpeech);
