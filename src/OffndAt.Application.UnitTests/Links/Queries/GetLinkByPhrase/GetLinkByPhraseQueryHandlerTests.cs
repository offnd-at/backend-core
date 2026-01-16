using Microsoft.Extensions.Logging;
using NSubstitute;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Links.Queries.GetLinkByPhrase;
using OffndAt.Application.Links.ReadModels;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.UnitTests.Links.Queries.GetLinkByPhrase;

internal sealed class GetLinkByPhraseQueryHandlerTests
{
    private GetLinkByPhraseQueryHandler _handler = null!;
    private ILinkQueryService _queryService = null!;

    [SetUp]
    public void Setup()
    {
        _queryService = Substitute.For<ILinkQueryService>();
        _handler = new GetLinkByPhraseQueryHandler(_queryService, Substitute.For<ILogger<GetLinkByPhraseQueryHandler>>());
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
        _queryService.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(Maybe<LinkReadModel>.None);

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase"), CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task Handle_ShouldReturnMaybeWithValue_WhenFoundLink()
    {
        var expected = new LinkReadModel
        {
            Id = Guid.NewGuid(),
            Phrase = "test-phrase",
            TargetUrl = "",
            LanguageId = 0,
            ThemeId = 0,
            VisitSummary = new LinkVisitSummaryReadModel
            {
                LinkId = Guid.NewGuid(),
                TotalVisits = 69
            },
            RecentEntries = [],
            CreatedAt = DateTimeOffset.Now
        };

        _queryService.GetByPhraseAsync(Arg.Any<Phrase>(), Arg.Any<CancellationToken>()).Returns(expected);

        var actual = await _handler.Handle(new GetLinkByPhraseQuery("test-phrase"), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value.Id, Is.EqualTo(expected.Id));
        });
    }
}
