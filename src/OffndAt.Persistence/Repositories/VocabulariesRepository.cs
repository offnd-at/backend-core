namespace OffndAt.Persistence.Repositories;

using Application.Core.Abstractions.Data;
using Constants;
using Core.Cache.Settings;
using Domain.Core.Primitives;
using Domain.Enumerations;
using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

/// <summary>
///     Represents the vocabularies repository.
/// </summary>
/// <param name="cacheOptions">The cache options.</param>
/// <param name="memoryCache">The memory cache.</param>
/// <param name="vocabularyLoader">The vocabulary loader.</param>
/// <param name="vocabularyService">The vocabulary service.</param>
internal sealed class VocabulariesRepository(
    IOptions<CacheSettings> cacheOptions,
    IMemoryCache memoryCache,
    IVocabularyLoader vocabularyLoader,
    IVocabularyService vocabularyService) : IVocabulariesRepository
{
    private readonly CacheSettings _settings = cacheOptions.Value;

    /// <inheritdoc />
    public async Task<Maybe<Vocabulary>> GetNounsAsync(
        Language language,
        Offensiveness offensiveness,
        Theme theme,
        CancellationToken cancellationToken = default)
    {
        var (number, gender) = vocabularyService.GenerateGrammaticalPropertiesForNounVocabulary(language, theme);

        return await memoryCache.GetOrCreateAsync(
                   CacheKeys.NounVocabulary(
                       language,
                       offensiveness,
                       number,
                       gender,
                       theme),
                   cacheEntry =>
                   {
                       cacheEntry.SetAbsoluteExpiration(_settings.LongTtl);

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
    }

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
                cacheEntry.SetAbsoluteExpiration(_settings.LongTtl);

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
                cacheEntry.SetAbsoluteExpiration(_settings.LongTtl);

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
