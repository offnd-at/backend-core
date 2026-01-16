using Microsoft.Extensions.Logging;
using NSubstitute;
using OffndAt.Application.Links.Queries.GetLinkByPhrase;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.UnitTests.Links.Queries.GetLinkByPhrase;

internal sealed class GetLinkByPhraseQueryHandlerTests
{
    private GetLinkByPhraseQueryHandler _handler = null!;
    private ILinkRepository _linkRepository = null!;

    [SetUp]
    public void Setup()
    {
        _linkRepository = Substitute.For<ILinkRepository>();

        _handler = new GetLinkByPhraseQueryHandler(_linkRepository, Substitute.For<ILogger<GetLinkByPhraseQueryHandler>>());
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyMaybe_WhenPhraseCreationFailed()
    {
        var actual = await _handler.Handle(new GetLinkByPhraseQuery(string.Empty), CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyMaybe_WhenDidNotFindLink()
    {
        _linkRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.None);

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase"), CancellationToken.None);

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

        _linkRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.From(link));

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase"), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value.Link.TargetUrl, Is.EqualTo(link.TargetUrl.Value));
        });
    }
}
