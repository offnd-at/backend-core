using MassTransit;
using NSubstitute;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Links.IntegrationEvents.LinkVisited;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.IntegrationEvents.LinkVisited;

internal sealed class TrackLinkVisitHandlerTests
{
    private TrackLinkVisitHandler _handler = null!;
    private ILinkVisitTracker _linkVisitTracker = null!;

    [SetUp]
    public void Setup()
    {
        _linkVisitTracker = Substitute.For<ILinkVisitTracker>();
        _handler = new TrackLinkVisitHandler(_linkVisitTracker);
    }

    [Test]
    public async Task Consume_ShouldRecordLinkVisit_InTracker()
    {
        var context = Substitute.For<ConsumeContext<LinkVisitedIntegrationEvent>>();
        var linkId = new LinkId(Guid.NewGuid());
        var domainEvent = new LinkVisitedDomainEvent(
            linkId,
            new LinkVisitedContext(Language.English, Theme.None, DateTimeOffset.UtcNow));
        var message = new LinkVisitedIntegrationEvent(domainEvent);
        context.Message.Returns(message);
        context.CancellationToken.Returns(TestContext.CurrentContext.CancellationToken);

        await _handler.Consume(context);

        await _linkVisitTracker.Received(1)
            .RecordAsync(
                Arg.Is<LinkId>(id => id.Value == linkId),
                Arg.Any<CancellationToken>());
    }
}
