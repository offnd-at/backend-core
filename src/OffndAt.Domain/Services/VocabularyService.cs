using Enumerations;


namespace OffndAt.Domain.Services;/// <summary>
///     Provides business logic for managing and validating vocabularies.
/// </summary>
internal sealed class VocabularyService : IVocabularyService
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public (GrammaticalNumber number, GrammaticalGender gender) GenerateGrammaticalPropertiesForNounVocabulary(
        Language language,
        Theme theme)
    {
        var availableNumbers = GetAvailableNumbers(language, theme);
        var number = availableNumbers[_random.Next(availableNumbers.Count)];

        var availableGenders = GetAvailableGenders(language, theme, number);
        var gender = availableGenders[_random.Next(availableGenders.Count)];

        return (number, gender);
    }

    private static List<GrammaticalNumber> GetAvailableNumbers(Language language, Theme theme)
    {
        if (language == Language.English)
        {
            return [GrammaticalNumber.None];
        }

        if (language == Language.Polish)
        {
            if (theme == Theme.Politicians)
            {
                return [GrammaticalNumber.Singular];
            }

            return [GrammaticalNumber.Singular, GrammaticalNumber.Plural];
        }

        return [GrammaticalNumber.None];
    }

    private static List<GrammaticalGender> GetAvailableGenders(Language language, Theme theme, GrammaticalNumber number)
    {
        if (language == Language.English)
        {
            return [GrammaticalGender.None];
        }

        if (language == Language.Polish)
        {
            if (number == GrammaticalNumber.Singular)
            {
                if (theme == Theme.Politicians)
                {
                    return [GrammaticalGender.Masculine, GrammaticalGender.Feminine];
                }

                return [GrammaticalGender.Masculine, GrammaticalGender.Feminine, GrammaticalGender.Neuter];
            }

            if (number == GrammaticalNumber.Plural)
            {
                return [GrammaticalGender.MasculinePersonal, GrammaticalGender.NonMasculinePersonal];
            }
        }

        return [GrammaticalGender.None];
    }
}
