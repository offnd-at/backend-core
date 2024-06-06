namespace OffndAt.Domain.Services;

using Enumerations;

/// <summary>
///     Represents the vocabulary service.
/// </summary>
public sealed class VocabularyService
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <summary>
    ///     Generates grammatical properties for a vocabulary.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <returns>A tuple containing randomly chosen grammatical number and grammatical gender.</returns>
    /// <exception cref="ArgumentOutOfRangeException">if the specified language is out of range.</exception>
    public (GrammaticalNumber number, GrammaticalGender gender) GenerateGrammaticalPropertiesForNounVocabulary(Language language)
    {
        if (language == Language.English)
        {
            return (GrammaticalNumber.None, GrammaticalGender.None);
        }

        if (language == Language.Polish)
        {
            var availableNumbers = new List<GrammaticalNumber>
            {
                GrammaticalNumber.Singular,
                GrammaticalNumber.Plural
            };
            var number = availableNumbers[_random.Next(availableNumbers.Count)];

            if (number == GrammaticalNumber.Singular)
            {
                var availableGenders = new List<GrammaticalGender>
                {
                    GrammaticalGender.Masculine,
                    GrammaticalGender.Feminine,
                    GrammaticalGender.Neuter
                };
                var gender = availableGenders[_random.Next(availableGenders.Count)];

                return (number, gender);
            }

            if (number == GrammaticalNumber.Plural)
            {
                var availableGenders = new List<GrammaticalGender>
                {
                    GrammaticalGender.MasculinePersonal,
                    GrammaticalGender.NonMasculinePersonal
                };
                var gender = availableGenders[_random.Next(availableGenders.Count)];

                return (number, gender);
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(language),
            language,
            "Could not generate grammatical properties for unknown language.");
    }
}
