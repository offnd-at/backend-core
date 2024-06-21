namespace OffndAt.Application.UnitTests.Links.Events.LinkVisited;

using Application.Links.Events.LinkVisited;
using Core.Abstractions.Messaging;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Events;
using Domain.ValueObjects;
using NSubstitute;

internal sealed class LinkVisitedDomainEventHandlerTests
{
    private LinkVisitedDomainEventHandler _handler = null!;
    private IIntegrationEventPublisher _integrationEventPublisher = null!;

    [SetUp]
    public void Setup()
    {
        _integrationEventPublisher = Substitute.For<IIntegrationEventPublisher>();

        _handler = new LinkVisitedDomainEventHandler(_integrationEventPublisher);
    }

    [Test]
    public async Task Handle_ShouldPublishIntegrationEvent()
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

        await _handler.Handle(new LinkVisitedDomainEvent(link), CancellationToken.None);

        await _integrationEventPublisher.Received()
            .PublishAsync(
                Arg.Is<LinkVisitedIntegrationEvent>(e => e.LinkId == link.Id && e.Visits == link.Visits),
                Arg.Any<CancellationToken>());
    }
}
