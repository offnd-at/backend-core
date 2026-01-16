using Microsoft.Extensions.Logging;
using NSubstitute;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Links.Commands.VisitLink;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Abstractions.Services;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.Commands.VisitLink;

internal sealed class VisitLinkCommandHandlerTests
{
    private VisitLinkCommandHandler _handler = null!;
    private ILinkCache _linkCache = null!;
    private ILinkRepository _linkRepository = null!;
    private ILinkService _linkService = null!;
    private ILogger<VisitLinkCommandHandler> _logger = null!;

    [SetUp]
    public void Setup()
    {
        _linkRepository = Substitute.For<ILinkRepository>();
        _linkCache = Substitute.For<ILinkCache>();
        _linkService = Substitute.For<ILinkService>();
        _logger = Substitute.For<ILogger<VisitLinkCommandHandler>>();

        _handler = new VisitLinkCommandHandler(
            _linkRepository,
            _linkCache,
            _linkService,
            _logger);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenPhraseCreationFailed()
    {
        var actual = await _handler.Handle(new VisitLinkCommand(string.Empty), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.NullOrEmpty));
        });
    }

    [Test]
    public async Task Handle_ShouldRecordVisitAndReturnTargetUrl_WhenLinkIsCached()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var cachedLink = new CachedLink(
            new LinkId(Guid.NewGuid()),
            targetUrl,
            Language.English,
            Theme.None);

        _linkCache.GetLinkAsync(phrase, Arg.Any<CancellationToken>()).Returns(cachedLink);

        var actual = await _handler.Handle(new VisitLinkCommand("test-phrase"), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value, Is.EqualTo(targetUrl));
        });

        await _linkService.Received(1)
            .RecordLinkVisitAsync(
                cachedLink.LinkId,
                Arg.Is<LinkVisitedContext>(ctx => ctx.Language == cachedLink.Language && ctx.Theme == cachedLink.Theme));
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenLinkNotFoundInRepository()
    {
        var phrase = Phrase.Create("test-phrase").Value;

        _linkCache.GetLinkAsync(phrase, Arg.Any<CancellationToken>()).Returns(Maybe<CachedLink>.None);
        _linkRepository.GetByPhraseAsync(phrase, Arg.Any<CancellationToken>()).Returns(Maybe<Link>.None);

        var actual = await _handler.Handle(new VisitLinkCommand("test-phrase"), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Link.NotFound));
        });
    }

    [Test]
    public async Task Handle_ShouldCacheLinkRecordVisitAndReturnTargetUrl_WhenLinkFoundInRepository()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var link = Link.Create(
            phrase,
            targetUrl,
            Language.English,
            Theme.None);

        _linkCache.GetLinkAsync(phrase, Arg.Any<CancellationToken>()).Returns(Maybe<CachedLink>.None);
        _linkRepository.GetByPhraseAsync(phrase, Arg.Any<CancellationToken>()).Returns(Maybe<Link>.From(link));

        var actual = await _handler.Handle(new VisitLinkCommand("test-phrase"), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value, Is.EqualTo(targetUrl));
        });

        await _linkCache.Received(1).SetLinkAsync(link, Arg.Any<CancellationToken>());
    }
}
