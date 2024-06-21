﻿namespace OffndAt.Application.UnitTests.Links.Queries.GetLinkByPhrase;

using Application.Links.Queries.GetLinkByPhrase;
using Core.Abstractions.Data;
using Domain.Core.Primitives;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Repositories;
using Domain.ValueObjects;
using NSubstitute;

internal sealed class GetLinkByPhraseQueryHandlerTests
{
    private GetLinkByPhraseQueryHandler _handler = null!;
    private ILinksRepository _linksRepository = null!;
    private IUnitOfWork _unitOfWork = null!;

    [SetUp]
    public void Setup()
    {
        _linksRepository = Substitute.For<ILinksRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _handler = new GetLinkByPhraseQueryHandler(_linksRepository, _unitOfWork);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyMaybe_WhenPhraseCreationFailed()
    {
        var actual = await _handler.Handle(new GetLinkByPhraseQuery(string.Empty, false), CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyMaybe_WhenDidNotFindLink()
    {
        _linksRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.None);

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase", false), CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task Handle_ShouldReturnMaybeWithValue_WhenFoundLink()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var language = Language.English;
        var theme = Theme.None;

        var link = Link.Create(
            phrase,
            targetUrl,
            language,
            theme);

        _linksRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.From(link));

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase", false), CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value.Link.TargetUrl, Is.EqualTo(link.TargetUrl.Value));
                Assert.That(actual.Value.Link.Visits, Is.EqualTo(link.Visits));
            });
    }

    [Test]
    public async Task Handle_ShouldNotIncrementVisitsCounter_WhenFoundLinkAndCounterIncrementWasNotRequested()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var language = Language.English;
        var theme = Theme.None;

        var link = Link.Create(
            phrase,
            targetUrl,
            language,
            theme);

        _linksRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.From(link));

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase", false), CancellationToken.None);

        await _unitOfWork.Received(0).SaveChangesAsync(Arg.Any<CancellationToken>());

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value.Link.TargetUrl, Is.EqualTo(link.TargetUrl.Value));
                Assert.That(actual.Value.Link.Visits, Is.EqualTo(0));
            });
    }

    [Test]
    public async Task Handle_ShouldIncrementVisitsCounter_WhenFoundLinkAndCounterIncrementWasRequested()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var language = Language.English;
        var theme = Theme.None;

        var link = Link.Create(
            phrase,
            targetUrl,
            language,
            theme);

        _linksRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.From(link));

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase", true), CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value.Link.TargetUrl, Is.EqualTo(link.TargetUrl.Value));
                Assert.That(actual.Value.Link.Visits, Is.EqualTo(1));
            });
    }
}
