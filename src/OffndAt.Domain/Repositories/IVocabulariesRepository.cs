namespace OffndAt.Domain.Repositories;

using Core.Primitives;
using Enumerations;
using ValueObjects;

/// <summary>
///     Defines the contract for vocabulary entity data access operations.
/// </summary>
public interface IVocabulariesRepository
{
    /// <summary>
    ///     Gets a vocabulary containing nouns with the specified parameters.
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
    ///     Gets a vocabulary containing adjectives with the specified parameters.
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
    ///     Gets a vocabulary containing adverbs with the specified parameters.
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
