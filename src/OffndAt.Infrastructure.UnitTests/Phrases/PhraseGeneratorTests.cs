using Application.Core.Abstractions.Words;
using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.Enumerations;
using Domain.Models;
using Domain.Repositories;
using Domain.ValueObjects;
using Infrastructure.Phrases;
using NSubstitute;


namespace OffndAt.Infrastructure.UnitTests.Phrases;internal sealed class PhraseGeneratorTests
{
    private readonly Vocabulary _vocabulary = Vocabulary.Create(
            new VocabularyDescriptor(
                Language.English,
                Theme.None,
                Offensiveness.Offensive,
                GrammaticalNumber.None,
                GrammaticalGender.None,
                PartOfSpeech.Adjective),
            [Word.Create("word").Value])
        .Value;

    private ICaseConverter _caseConverter = null!;
    private PhraseGenerator _phraseGenerator = null!;
    private IVocabulariesRepository _vocabulariesRepository = null!;

    [SetUp]
    public void Setup()
    {
        _vocabulariesRepository = Substitute.For<IVocabulariesRepository>();
        _caseConverter = Substitute.For<ICaseConverter>();

        _phraseGenerator = new PhraseGenerator(_vocabulariesRepository, _caseConverter);
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenCouldNotFindNounsVocabulary()
    {
        _vocabulariesRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.None);

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsSuccess, Is.False);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.NotFound));
            });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenCouldNotFindAdjectivesVocabulary()
    {
        _vocabulariesRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.None);

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsSuccess, Is.False);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.NotFound));
            });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenCouldNotFindAdverbsVocabulary()
    {
        _vocabulariesRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdverbsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.None);

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsSuccess, Is.False);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.NotFound));
            });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenPhraseCreationFailed()
    {
        _vocabulariesRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdverbsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _caseConverter.Convert(
                Arg.Any<Format>(),
                Arg.Any<Word>(),
                Arg.Any<Word>(),
                Arg.Any<Word>())
            .Returns(string.Empty);

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsSuccess, Is.False);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.NullOrEmpty));
            });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnPhrase_WhenGenerationWasSuccessful()
    {
        _vocabulariesRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabulariesRepository.GetAdverbsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _caseConverter.Convert(
                Arg.Any<Format>(),
                Arg.Any<Word>(),
                Arg.Any<Word>(),
                Arg.Any<Word>())
            .Returns("test-phrase");

        var expected = Phrase.Create("test-phrase").Value;

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsSuccess, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });
    }
}
