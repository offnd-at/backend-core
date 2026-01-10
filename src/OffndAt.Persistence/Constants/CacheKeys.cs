using OffndAt.Domain.Enumerations;

namespace OffndAt.Persistence.Constants;

/// <summary>
///     Contains the cache keys constants.
/// </summary>
internal static class CacheKeys
{
    /// <summary>
    ///     Generates the cache key for vocabulary of nouns.
    /// </summary>
    internal static Func<Language, Offensiveness, GrammaticalNumber, GrammaticalGender, Theme, string> NounVocabulary { get; } =
        (
                language,
                offensiveness,
                number,
                gender,
                theme) =>
            $"vocabulary:noun:" +
            $"{language.Code.ToLowerInvariant()}:" +
            $"{offensiveness.Name.ToLowerInvariant()}:" +
            $"{number.Name.ToLowerInvariant()}:" +
            $"{gender.Name.ToLowerInvariant()}:" +
            $"{theme.Name.ToLowerInvariant()}";

    /// <summary>
    ///     Generates the cache key for vocabulary of adjectives.
    /// </summary>
    internal static Func<Language, Offensiveness, GrammaticalNumber, GrammaticalGender, string> AdjectiveVocabulary { get; } =
        (
                language,
                offensiveness,
                number,
                gender) =>
            $"vocabulary:adjective:" +
            $"{language.Code.ToLowerInvariant()}:" +
            $"{offensiveness.Name.ToLowerInvariant()}:" +
            $"{number.Name.ToLowerInvariant()}:" +
            $"{gender.Name.ToLowerInvariant()}";

    /// <summary>
    ///     Generates the cache key for vocabulary of adverbs.
    /// </summary>
    internal static Func<Language, Offensiveness, string> AdverbVocabulary { get; } = (language, offensiveness) =>
        $"vocabulary:adverb:" +
        $"{language.Code.ToLowerInvariant()}:" +
        $"{offensiveness.Name.ToLowerInvariant()}";
}
