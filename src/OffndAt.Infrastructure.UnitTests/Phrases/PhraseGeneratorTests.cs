using NSubstitute;
using OffndAt.Application.Abstractions.Words;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Phrases;

namespace OffndAt.Infrastructure.UnitTests.Phrases;

internal sealed class PhraseGeneratorTests
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
    private IVocabularyRepository _vocabularyRepository = null!;

    [SetUp]
    public void Setup()
    {
        _vocabularyRepository = Substitute.For<IVocabularyRepository>();
        _caseConverter = Substitute.For<ICaseConverter>();

        _phraseGenerator = new PhraseGenerator(_vocabularyRepository, _caseConverter);
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenCouldNotFindNounsVocabulary()
    {
        _vocabularyRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.None);

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.False);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.NotFound));
        });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenCouldNotFindAdjectivesVocabulary()
    {
        _vocabularyRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdjectivesAsync(
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
            TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.False);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.NotFound));
        });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenCouldNotFindAdverbsVocabulary()
    {
        _vocabularyRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdverbsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.None);

        var actual = await _phraseGenerator.GenerateAsync(
            Format.KebabCase,
            Language.English,
            Theme.None,
            TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.False);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.NotFound));
        });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnError_WhenPhraseCreationFailed()
    {
        _vocabularyRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdverbsAsync(
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
            TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.False);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.NullOrEmpty));
        });
    }

    [Test]
    public async Task GenerateAsync_ShouldReturnPhrase_WhenGenerationWasSuccessful()
    {
        _vocabularyRepository.GetNounsAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdjectivesAsync(
                Arg.Any<Language>(),
                Arg.Any<Offensiveness>(),
                Arg.Any<GrammaticalNumber>(),
                Arg.Any<GrammaticalGender>(),
                Arg.Any<CancellationToken>())
            .Returns(_vocabulary);

        _vocabularyRepository.GetAdverbsAsync(
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
            TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value, Is.EqualTo(expected));
        });
    }
}
