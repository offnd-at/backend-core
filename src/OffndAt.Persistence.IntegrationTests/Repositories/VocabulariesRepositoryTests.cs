using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Models;
using OffndAt.Domain.Services;
using OffndAt.Domain.ValueObjects;
using OffndAt.Persistence.Core.Cache.Settings;
using OffndAt.Persistence.Repositories;

namespace OffndAt.Persistence.IntegrationTests.Repositories;

internal sealed class VocabulariesRepositoryTests
{
    private IOptions<CacheSettings> _cacheSettingsOptions = null!;
    private VocabulariesRepository _repository = null!;
    private IVocabularyLoader _vocabularyLoader = null!;
    private IVocabularyService _vocabularyService = null!;

    [SetUp]
    public void Setup()
    {
        _cacheSettingsOptions = Substitute.For<IOptions<CacheSettings>>();
        _cacheSettingsOptions.Value.Returns(
            new CacheSettings
            {
                LongTtl = TimeSpan.FromMilliseconds(1),
                ShortTtl = TimeSpan.FromMilliseconds(1)
            });

        _vocabularyLoader = Substitute.For<IVocabularyLoader>();
        _vocabularyService = Substitute.For<IVocabularyService>();
        _vocabularyService.GenerateGrammaticalPropertiesForNounVocabulary(Arg.Any<Language>(), Arg.Any<Theme>())
            .Returns((GrammaticalNumber.None, GrammaticalGender.None));

        _repository = new VocabulariesRepository(
            _cacheSettingsOptions,
            new MemoryCache(new MemoryCacheOptions()),
            _vocabularyLoader,
            _vocabularyService);
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

        Assert.Multiple(() =>
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

        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value, Is.EqualTo(expected));
        });

        var actual2 = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual2.HasValue, Is.True);
            Assert.That(actual2.Value, Is.EqualTo(expected));
        });

        Assert.That(actual, Is.SameAs(actual2));

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetNounsAsync_ShouldNotReturnVocabularyFromCache_WhenDifferentRandomGrammaticalPropertiesWereDrawn()
    {
        var descriptor = new VocabularyDescriptor(
            Language.English,
            Theme.None,
            Offensiveness.NonOffensive,
            GrammaticalNumber.None,
            GrammaticalGender.None,
            PartOfSpeech.Noun);

        var vocabulary1 = Vocabulary.Create(descriptor, [Word.Create("test-word-1").Value]).Value;
        var expected = Vocabulary.Create(descriptor, [Word.Create("test-word-2").Value]).Value;

        _vocabularyLoader.DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>())
            .Returns(_ => Maybe<Vocabulary>.From(vocabulary1), _ => Maybe<Vocabulary>.From(expected));

        _vocabularyService.GenerateGrammaticalPropertiesForNounVocabulary(Arg.Any<Language>(), Arg.Any<Theme>())
            .Returns(_ => (GrammaticalNumber.None, GrammaticalGender.None), _ => (GrammaticalNumber.Singular, GrammaticalGender.Masculine));

        var actual = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value, Is.EqualTo(vocabulary1));
        });

        var actual2 = await _repository.GetNounsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            Theme.None,
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual2.HasValue, Is.True);
            Assert.That(actual2.Value, Is.EqualTo(expected));
        });

        Assert.That(actual, Is.Not.SameAs(actual2));

        await _vocabularyLoader.Received(2).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());
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

        Assert.Multiple(() =>
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

        Assert.Multiple(() =>
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

        Assert.Multiple(() =>
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

        Assert.Multiple(() =>
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

        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value, Is.EqualTo(expected));
        });

        var actual2 = await _repository.GetAdverbsAsync(
            Language.English,
            Offensiveness.NonOffensive,
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual2.HasValue, Is.True);
            Assert.That(actual2.Value, Is.EqualTo(expected));
        });

        Assert.That(actual, Is.SameAs(actual2));

        await _vocabularyLoader.Received(1).DownloadAsync(Arg.Any<VocabularyDescriptor>(), Arg.Any<CancellationToken>());
    }
}
