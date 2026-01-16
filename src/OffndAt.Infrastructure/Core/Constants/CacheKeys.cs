using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Infrastructure.Core.Constants;

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

    /// <summary>
    ///     Generates the cache key for the target URL of a link.
    /// </summary>
    internal static Func<Phrase, string> LinkTargetUrl { get; } = phrase => $"link:target:{phrase.Value}";

    /// <summary>
    ///     Generates the cache key for tracking the number of visits associated with a specific link.
    /// </summary>
    internal static Func<LinkId, string> LinkVisits { get; } = linkId => $"link:visits:{linkId}";
}
