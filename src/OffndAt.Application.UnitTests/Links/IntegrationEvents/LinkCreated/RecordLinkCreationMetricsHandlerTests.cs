using MassTransit;
using NSubstitute;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Application.Links.IntegrationEvents.LinkCreated;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.IntegrationEvents.LinkCreated;

internal sealed class RecordLinkCreationMetricsHandlerTests
{
    private RecordLinkCreationMetricsHandler _handler = null!;
    private ILinkMetrics _linkMetrics = null!;

    [SetUp]
    public void Setup()
    {
        _linkMetrics = Substitute.For<ILinkMetrics>();
        _handler = new RecordLinkCreationMetricsHandler(_linkMetrics);
    }

    [Test]
    public async Task Consume_ShouldRecordLinkCreation_WithCorrectParameters()
    {
        var context = Substitute.For<ConsumeContext<LinkCreatedIntegrationEvent>>();
        var domainEvent = new LinkCreatedDomainEvent(
            new LinkId(Guid.NewGuid()),
            Language.English,
            Theme.None);
        var message = new LinkCreatedIntegrationEvent(domainEvent);
        context.Message.Returns(message);

        await _handler.Consume(context);

        _linkMetrics.Received(1).RecordLinkCreation(Language.English, Theme.None);
    }
}
