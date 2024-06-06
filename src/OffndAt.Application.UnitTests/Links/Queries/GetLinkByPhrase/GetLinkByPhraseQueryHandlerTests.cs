namespace OffndAt.Application.UnitTests.Links.Queries.GetLinkByPhrase;

using Application.Links.Queries.GetLinkByPhrase;
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

    [SetUp]
    public void Setup()
    {
        _linksRepository = Substitute.For<ILinksRepository>();

        _handler = new GetLinkByPhraseQueryHandler(_linksRepository);
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
        _linksRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.None);

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

        _linksRepository.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<Link>.From(link));

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase"), CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value.Link.TargetUrl, Is.EqualTo(link.TargetUrl.Value));
                Assert.That(actual.Value.Link.Visits, Is.EqualTo(link.Visits));
            });
    }
}
