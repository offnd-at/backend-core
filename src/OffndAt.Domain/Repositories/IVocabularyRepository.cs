using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Domain.Repositories;

/// <summary>
///     Represents the vocabulary repository interface.
/// </summary>
public interface IVocabularyRepository
{
    /// <summary>
    ///     Gets vocabulary containing nouns with the specified parameters.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="offensiveness">The offensiveness.</param>
    /// <param name="theme">The theme.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the vocabulary with the specified parameters.</returns>
    Task<Maybe<Vocabulary>> GetNounsAsync(
        Language language,
        Offensiveness offensiveness,
        Theme theme,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets vocabulary containing adjectives with the specified parameters.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="offensiveness">The offensiveness.</param>
    /// <param name="number">The grammatical number.</param>
    /// <param name="gender">The grammatical gender.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the vocabulary with the specified parameters.</returns>
    Task<Maybe<Vocabulary>> GetAdjectivesAsync(
        Language language,
        Offensiveness offensiveness,
        GrammaticalNumber number,
        GrammaticalGender gender,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets vocabulary containing adverbs with the specified parameters.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="offensiveness">The offensiveness.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the vocabulary with the specified parameters.</returns>
    Task<Maybe<Vocabulary>> GetAdverbsAsync(
        Language language,
        Offensiveness offensiveness,
        CancellationToken cancellationToken = default);
}
