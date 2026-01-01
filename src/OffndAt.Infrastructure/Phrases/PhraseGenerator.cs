using OffndAt.Application.Core.Abstractions.Phrases;
using OffndAt.Application.Core.Abstractions.Words;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Infrastructure.Phrases;

/// <summary>
///     Represents the phrase generator.
/// </summary>
/// <param name="vocabulariesRepository">The vocabularies repository.</param>
/// <param name="caseConverter">The case converter.</param>
internal sealed class PhraseGenerator(IVocabulariesRepository vocabulariesRepository, ICaseConverter caseConverter) : IPhraseGenerator
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public async Task<Result<Phrase>> GenerateAsync(
        Format format,
        Language language,
        Theme theme,
        CancellationToken cancellationToken = default)
    {
        var nounOffensiveness = Offensiveness.FromBoolean(theme == Theme.None && _random.NextDouble() >= 0.5);
        var adjectiveOffensiveness =
            Offensiveness.FromBoolean(nounOffensiveness == Offensiveness.NonOffensive && _random.NextDouble() >= 0.5);
        var adverbOffensiveness = Offensiveness.FromBoolean(
            nounOffensiveness == Offensiveness.NonOffensive && adjectiveOffensiveness == Offensiveness.NonOffensive);

        var maybeNounVocabulary = await vocabulariesRepository.GetNounsAsync(
            language,
            nounOffensiveness,
            theme,
            cancellationToken);

        if (maybeNounVocabulary.HasNoValue)
        {
            return Result.Failure<Phrase>(DomainErrors.Vocabulary.NotFound);
        }

        var maybeAdjectiveVocabulary = await vocabulariesRepository.GetAdjectivesAsync(
            language,
            adjectiveOffensiveness,
            maybeNounVocabulary.Value.GrammaticalNumber,
            maybeNounVocabulary.Value.GrammaticalGender,
            cancellationToken);

        if (maybeAdjectiveVocabulary.HasNoValue)
        {
            return Result.Failure<Phrase>(DomainErrors.Vocabulary.NotFound);
        }

        var maybeAdverbVocabulary = await vocabulariesRepository.GetAdverbsAsync(language, adverbOffensiveness, cancellationToken);

        if (maybeAdverbVocabulary.HasNoValue)
        {
            return Result.Failure<Phrase>(DomainErrors.Vocabulary.NotFound);
        }

        var phraseString = caseConverter.Convert(
            format,
            maybeAdverbVocabulary.Value.RandomWord,
            maybeAdjectiveVocabulary.Value.RandomWord,
            maybeNounVocabulary.Value.RandomWord);

        var phraseResult = Phrase.Create(phraseString);

        return phraseResult.IsFailure
            ? Result.Failure<Phrase>(phraseResult.Error)
            : Result.Success(phraseResult.Value);
    }
}
