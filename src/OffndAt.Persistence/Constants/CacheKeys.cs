using Domain.Enumerations;


namespace OffndAt.Persistence.Constants;/// <summary>
///     Contains the cache keys constants.
/// </summary>
internal static class CacheKeys
{
    /// <summary>
    ///     Generates the cache key for a noun vocabulary.
    /// </summary>
    internal static Func<Language, Offensiveness, GrammaticalNumber, GrammaticalGender, Theme, string> NounVocabulary { get; } =
        (
                language,
                offensiveness,
                number,
                gender,
                theme) =>
            $"noun-vocabulary-" +
            $"{language.Code.ToLowerInvariant()}-" +
            $"{offensiveness.Name.ToLowerInvariant()}-" +
            $"{number.Name.ToLowerInvariant()}-" +
            $"{gender.Name.ToLowerInvariant()}" +
            $"{theme.Name.ToLowerInvariant()}";

    /// <summary>
    ///     Generates the cache key for an adjective vocabulary.
    /// </summary>
    internal static Func<Language, Offensiveness, GrammaticalNumber, GrammaticalGender, string> AdjectiveVocabulary { get; } =
        (
                language,
                offensiveness,
                number,
                gender) =>
            $"adjective-vocabulary-" +
            $"{language.Code.ToLowerInvariant()}-" +
            $"{offensiveness.Name.ToLowerInvariant()}-" +
            $"{number.Name.ToLowerInvariant()}-" +
            $"{gender.Name.ToLowerInvariant()}";

    /// <summary>
    ///     Generates the cache key for an adverb vocabulary.
    /// </summary>
    internal static Func<Language, Offensiveness, string> AdverbVocabulary { get; } = (language, offensiveness) =>
        $"adverb-vocabulary-" +
        $"{language.Code.ToLowerInvariant()}-" +
        $"{offensiveness.Name.ToLowerInvariant()}";
}
