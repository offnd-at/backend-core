namespace OffndAt.Persistence.Repositories;

using Application.Core.Abstractions.Data;
using Constants;
using Domain.Core.Primitives;
using Domain.Enumerations;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;

/// <summary>
///     Represents the vocabularies repository.
/// </summary>
/// <param name="memoryCache">The memory cache.</param>
/// <param name="vocabularyLoader">The vocabulary loader.</param>
internal sealed class VocabulariesRepository(IMemoryCache memoryCache, IVocabularyLoader vocabularyLoader) : IVocabulariesRepository
{
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(60);

    /// <inheritdoc />
    public async Task<Maybe<Vocabulary>> GetNounsAsync(
        Language language,
        Offensiveness offensiveness,
        Theme theme,
        CancellationToken cancellationToken = default) =>
        await memoryCache.GetOrCreateAsync(
            CacheKeys.NounVocabulary(language, offensiveness, theme),
            cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(_cacheTtl);

                var (number, gender) = new VocabularyService().GenerateGrammaticalPropertiesForNounVocabulary(language);

                var vocabularyDescriptor = new VocabularyDescriptor(
                    language,
                    theme,
                    offensiveness,
                    number,
                    gender,
                    PartOfSpeech.Noun);

                return vocabularyLoader.DownloadAsync(vocabularyDescriptor, cancellationToken);
            }) ??
        Maybe<Vocabulary>.None;

    /// <inheritdoc />
    public async Task<Maybe<Vocabulary>> GetAdjectivesAsync(
        Language language,
        Offensiveness offensiveness,
        GrammaticalNumber number,
        GrammaticalGender gender,
        CancellationToken cancellationToken = default) =>
        await memoryCache.GetOrCreateAsync(
            CacheKeys.AdjectiveVocabulary(
                language,
                offensiveness,
                number,
                gender),
            cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(_cacheTtl);

                var vocabularyDescriptor = new VocabularyDescriptor(
                    language,
                    Theme.None,
                    offensiveness,
                    number,
                    gender,
                    PartOfSpeech.Adjective);

                return vocabularyLoader.DownloadAsync(vocabularyDescriptor, cancellationToken);
            }) ??
        Maybe<Vocabulary>.None;

    /// <inheritdoc />
    public async Task<Maybe<Vocabulary>> GetAdverbsAsync(
        Language language,
        Offensiveness offensiveness,
        CancellationToken cancellationToken = default) =>
        await memoryCache.GetOrCreateAsync(
            CacheKeys.AdverbVocabulary(language, offensiveness),
            cacheEntry =>
            {
                cacheEntry.SetAbsoluteExpiration(_cacheTtl);

                var vocabularyDescriptor = new VocabularyDescriptor(
                    language,
                    Theme.None,
                    offensiveness,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Adverb);

                return vocabularyLoader.DownloadAsync(vocabularyDescriptor, cancellationToken);
            }) ??
        Maybe<Vocabulary>.None;
}
