using NSubstitute;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Data;
using Language = OffndAt.Domain.Enumerations.Language;

namespace OffndAt.Infrastructure.UnitTests.Core.Data;

internal sealed class GitHubVocabularyLoaderTests
{
    private readonly VocabularyDescriptor _descriptor = new(
        Language.English,
        Theme.Politicians,
        Offensiveness.Offensive,
        GrammaticalNumber.Plural,
        GrammaticalGender.MasculinePersonal,
        PartOfSpeech.Noun);

    private IFileLoader _fileLoader = null!;
    private GitHubVocabularyLoader _loader = null!;

    [SetUp]
    public void Setup()
    {
        _fileLoader = Substitute.For<IFileLoader>();

        _loader = new GitHubVocabularyLoader(_fileLoader);
    }

    [Test]
    public async Task DownloadAsync_ShouldCallFileLoaderWithPathBuiltFromVocabularyDescriptor()
    {
        _fileLoader.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Maybe<byte[]>.None);

        const string expectedPath = "/en/offensive/plural/masculine-personal/politicians/nouns.txt";

        await _loader.DownloadAsync(_descriptor);

        await _fileLoader.Received(1).DownloadAsync(Arg.Is<string>(value => value == expectedPath), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task DownloadAsync_ShouldReturnEmptyMaybe_WhenFileLoaderResponseHasNoValue()
    {
        _fileLoader.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Maybe<byte[]>.None);

        var actual = await _loader.DownloadAsync(_descriptor);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task DownloadAsync_ShouldReturnEmptyMaybe_WhenVocabularyCreationFailed()
    {
        var bytes = ""u8.ToArray();
        _fileLoader.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Maybe<byte[]>.From(bytes));

        var actual = await _loader.DownloadAsync(_descriptor);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task DownloadAsync_ShouldReadWordsFromFileAndAddThemToVocabulary_WhenSuccessfullyReadFile()
    {
        var bytes = "word1\nword2"u8.ToArray();
        _fileLoader.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Maybe<byte[]>.From(bytes));

        var actual = await _loader.DownloadAsync(_descriptor);
        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value.Words, Has.Count.EqualTo(2));
            Assert.That(actual.Value.Words[0].Value, Is.EqualTo("word1"));
            Assert.That(actual.Value.Words[1].Value, Is.EqualTo("word2"));
        });
    }
}
