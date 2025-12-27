using Application.Links.Commands.GenerateLink;
using Core.Abstractions.Phrases;
using Core.Abstractions.Urls;
using Core.Constants;
using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Polly;
using Polly.Registry;


namespace OffndAt.Application.UnitTests.Links.Commands.GenerateLink;internal sealed class GenerateLinkCommandHandlerTests
{
    private GenerateLinkCommandHandler _handler = null!;
    private ILinksRepository _linksRepository = null!;
    private ILogger<GenerateLinkCommandHandler> _logger = null!;
    private IPhraseGenerator _phraseGenerator = null!;
    private ResiliencePipelineProvider<string> _resiliencePipelineProvider = null!;
    private IUrlMaker _urlMaker = null!;

    [SetUp]
    public void Setup()
    {
        _linksRepository = Substitute.For<ILinksRepository>();
        _phraseGenerator = Substitute.For<IPhraseGenerator>();
        _urlMaker = Substitute.For<IUrlMaker>();
        _logger = Substitute.For<ILogger<GenerateLinkCommandHandler>>();

        _resiliencePipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();
        _resiliencePipelineProvider.GetPipeline<Result<Phrase>>(ResiliencePolicies.PhraseGeneratorRetryPolicyName)
            .Returns(ResiliencePipeline<Result<Phrase>>.Empty);

        _handler = new GenerateLinkCommandHandler(
            _linksRepository,
            _phraseGenerator,
            _urlMaker,
            _resiliencePipelineProvider,
            _logger);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenTargetUrlCreationFailed()
    {
        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                string.Empty,
                0,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Url.NullOrEmpty));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenFormatCreationFailed()
    {
        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                -1),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Format.NotFound));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenLanguageCreationFailed()
    {
        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                -1,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Language.NotFound));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenThemeCreationFailed()
    {
        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                -1,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Theme.NotFound));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenPhraseGenerationFailed()
    {
        var expectedError = new Error("test-code", "test-message");

        _phraseGenerator.GenerateAsync(
                Arg.Any<Format>(),
                Arg.Any<Language>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Phrase>(expectedError));

        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(expectedError));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenGeneratedPhraseIsAlreadyInUse()
    {
        _phraseGenerator.GenerateAsync(
                Arg.Any<Format>(),
                Arg.Any<Language>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success(Phrase.Create("test-phrase").Value));

        _linksRepository.HasAnyWithPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(true);

        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.AlreadyInUse));
            });
    }

    [Test]
    public async Task Handle_ShouldInsertNewLinkToTheDatabase_WhenPhraseWasSuccessfullyGenerated()
    {
        var expectedPhrase = Phrase.Create("test-phrase").Value;

        _phraseGenerator.GenerateAsync(
                Arg.Any<Format>(),
                Arg.Any<Language>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success(expectedPhrase));

        _linksRepository.HasAnyWithPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(false);

        _urlMaker.MakeRedirectUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Success(Url.Create("https://example.com").Value));
        _urlMaker.MakeStatsUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Success(Url.Create("https://example.com/stats").Value));

        _ = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                0),
            CancellationToken.None);

        _linksRepository.Received().Insert(Arg.Is<Link>(link => link.Phrase == expectedPhrase));
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenRedirectUrlGenerationFailed()
    {
        var expectedError = new Error("test-code", "test-message");

        _phraseGenerator.GenerateAsync(
                Arg.Any<Format>(),
                Arg.Any<Language>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success(Phrase.Create("test-phrase").Value));

        _linksRepository.HasAnyWithPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(false);

        _urlMaker.MakeRedirectUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Failure<Url>(expectedError));

        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(expectedError));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenStatsUrlGenerationFailed()
    {
        var expectedError = new Error("test-code", "test-message");

        _phraseGenerator.GenerateAsync(
                Arg.Any<Format>(),
                Arg.Any<Language>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success(Phrase.Create("test-phrase").Value));

        _linksRepository.HasAnyWithPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(false);

        _urlMaker.MakeRedirectUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Success(Url.Create("https://example.com").Value));
        _urlMaker.MakeStatsUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Failure<Url>(expectedError));

        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsFailure, Is.True);
                Assert.That(actual.Error, Is.EqualTo(expectedError));
            });
    }

    [Test]
    public async Task Handle_ShouldReturnResponse_WhenPhraseAndUrlsWereSuccessfullyGenerated()
    {
        _phraseGenerator.GenerateAsync(
                Arg.Any<Format>(),
                Arg.Any<Language>(),
                Arg.Any<Theme>(),
                Arg.Any<CancellationToken>())
            .Returns(Result.Success(Phrase.Create("test-phrase").Value));

        _linksRepository.HasAnyWithPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(false);

        _urlMaker.MakeRedirectUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Success(Url.Create("https://example.com").Value));
        _urlMaker.MakeStatsUrlForPhrase(Arg.Any<Phrase>()).Returns(Result.Success(Url.Create("https://example.com/stats").Value));

        var actual = await _handler.Handle(
            new GenerateLinkCommand(
                "https://example.com",
                0,
                0,
                0),
            CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.IsSuccess, Is.True);
                Assert.That(actual.Value.Url, Is.EqualTo("https://example.com"));
                Assert.That(actual.Value.StatsUrl, Is.EqualTo("https://example.com/stats"));
            });
    }
}
