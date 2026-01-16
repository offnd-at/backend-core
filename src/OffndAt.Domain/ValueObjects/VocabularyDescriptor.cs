using OffndAt.Domain.Enumerations;

namespace OffndAt.Domain.ValueObjects;

/// <summary>
///     Represents a value object that describes the properties of vocabulary.
/// </summary>
/// <param name="Language">The language.</param>
/// <param name="Theme">The theme.</param>
/// <param name="Offensiveness">The offensiveness.</param>
/// <param name="GrammaticalNumber">The grammatical number.</param>
/// <param name="GrammaticalGender">The grammatical gender.</param>
/// <param name="PartOfSpeech">The part of speech.</param>
public sealed record VocabularyDescriptor(
    Language Language,
    Theme Theme,
    Offensiveness Offensiveness,
    GrammaticalNumber GrammaticalNumber,
    GrammaticalGender GrammaticalGender,
    PartOfSpeech PartOfSpeech);
