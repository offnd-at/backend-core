namespace OffndAt.Persistence.IntegrationTests.Repositories;

using Application.Core.Abstractions.Data;
using Domain.Core.Primitives;
using Domain.Enumerations;
using Domain.Models;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Persistence.Repositories;

internal sealed class VocabulariesRepositoryTests
{
    private VocabulariesRepository _repository = null!;
    private IVocabularyLoader _vocabularyLoader = null!;

    [SetUp]
    public void Setup()
    {
        _vocabularyLoader = Substitute.For<IVocabularyLoader>();

        _repository = new VocabulariesRepository(new MemoryCache(new MemoryCacheOptions()), _vocabularyLoader);
    }

    [Test]
    public async Task GetNounsAsync_ShouldReturnEmptyMaybe_WhenReceivedNullFromLoader()
    {
        var actual = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task GetNounsAsync_ShouldReturnVocabulary_WhenItWasSuccessfullyLoaded()
    {
        var expected = Vocabulary.Create(
                new VocabularyDescriptor(
                    Language.English,
                    Theme.None,
                    Offensiveness.NonOffensive,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Noun),
                [Word.Create("test-word").Value])
            .Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.From(expected));

        var actual = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });
    }

    [Test]
    public async Task GetNounsAsync_ShouldReturnVocabularyFromCache_WhenItWasAlreadyLoadedBefore()
    {
        var expected = Vocabulary.Create(
                new VocabularyDescriptor(
                    Language.English,
                    Theme.None,
                    Offensiveness.NonOffensive,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Noun),
                [Word.Create("test-word").Value])
            .Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.From(expected));

        var actual = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });

        var actual2 = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual2.HasValue, Is.True);
                Assert.That(actual2.Value, Is.EqualTo(expected));
            });

        Assert.That(actual, Is.SameAs(actual2));

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetAdjectivesAsync_ShouldReturnEmptyMaybe_WhenReceivedNullFromLoader()
    {
        var actual = await _repository.GetAdjectivesAsync(
            Language.English,
            Offensiveness.NonOffensive,
            GrammaticalNumber.None,
            GrammaticalGender.None,
            CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task GetAdjectivesAsync_ShouldReturnVocabulary_WhenItWasSuccessfullyLoaded()
    {
        var expected = Vocabulary.Create(
                new VocabularyDescriptor(
                    Language.English,
                    Theme.None,
                    Offensiveness.NonOffensive,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Adjective),
                [Word.Create("test-word").Value])
            .Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.From(expected));

        var actual = await _repository.GetAdjectivesAsync(
            Language.English,
            Offensiveness.NonOffensive,
            GrammaticalNumber.None,
            GrammaticalGender.None,
            CancellationToken.None);

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });
    }

    [Test]
    public async Task GetAdjectivesAsync_ShouldReturnVocabularyFromCache_WhenItWasAlreadyLoadedBefore()
    {
        var expected = Vocabulary.Create(
                new VocabularyDescriptor(
                    Language.English,
                    Theme.None,
                    Offensiveness.NonOffensive,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Adjective),
                [Word.Create("test-word").Value])
            .Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.From(expected));

        var actual = await _repository.GetAdjectivesAsync(
            Language.English,
            Offensiveness.NonOffensive,
            GrammaticalNumber.None,
            GrammaticalGender.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });

        var actual2 = await _repository.GetAdjectivesAsync(
            Language.English,
            Offensiveness.NonOffensive,
            GrammaticalNumber.None,
            GrammaticalGender.None,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual2.HasValue, Is.True);
                Assert.That(actual2.Value, Is.EqualTo(expected));
            });

        Assert.That(actual, Is.SameAs(actual2));

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetAdverbsAsync_ShouldReturnEmptyMaybe_WhenReceivedNullFromLoader()
    {
        var actual = await _repository.GetAdverbsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task GetAdverbsAsync_ShouldReturnVocabulary_WhenItWasSuccessfullyLoaded()
    {
        var expected = Vocabulary.Create(
                new VocabularyDescriptor(
                    Language.English,
                    Theme.None,
                    Offensiveness.NonOffensive,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Adverb),
                [Word.Create("test-word").Value])
            .Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.From(expected));

        var actual = await _repository.GetAdverbsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            CancellationToken.None);

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });
    }

    [Test]
    public async Task GetAdverbsAsync_ShouldReturnVocabularyFromCache_WhenItWasAlreadyLoadedBefore()
    {
        var expected = Vocabulary.Create(
                new VocabularyDescriptor(
                    Language.English,
                    Theme.None,
                    Offensiveness.NonOffensive,
                    GrammaticalNumber.None,
                    GrammaticalGender.None,
                    PartOfSpeech.Adverb),
                [Word.Create("test-word").Value])
            .Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(Maybe<Vocabulary>.From(expected));

        var actual = await _repository.GetAdverbsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expected));
            });

        var actual2 = await _repository.GetAdverbsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual2.HasValue, Is.True);
                Assert.That(actual2.Value, Is.EqualTo(expected));
            });

        Assert.That(actual, Is.SameAs(actual2));

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());
    }
}
