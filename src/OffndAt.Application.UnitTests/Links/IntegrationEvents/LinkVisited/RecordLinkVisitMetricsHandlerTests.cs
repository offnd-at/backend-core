using MassTransit;
using NSubstitute;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Application.Links.IntegrationEvents.LinkVisited;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.IntegrationEvents.LinkVisited;

internal sealed class RecordLinkVisitMetricsHandlerTests
{
    private RecordLinkVisitMetricsHandler _handler = null!;
    private ILinkMetrics _linkMetrics = null!;

    [SetUp]
    public void Setup()
    {
        _linkMetrics = Substitute.For<ILinkMetrics>();
        _handler = new RecordLinkVisitMetricsHandler(_linkMetrics);
    }

    [Test]
    public async Task Consume_ShouldRecordLinkVisit_WithCorrectParameters()
    {
        var context = Substitute.For<ConsumeContext<LinkVisitedIntegrationEvent>>();
        var domainEvent = new LinkVisitedDomainEvent(
            new LinkId(Guid.NewGuid()),
            new LinkVisitedContext(Language.English, Theme.None, DateTimeOffset.UtcNow));
        var message = new LinkVisitedIntegrationEvent(domainEvent);
        context.Message.Returns(message);

        await _handler.Consume(context);

        _linkMetrics.Received(1).RecordLinkVisit(Language.English, Theme.None);
    }
}
