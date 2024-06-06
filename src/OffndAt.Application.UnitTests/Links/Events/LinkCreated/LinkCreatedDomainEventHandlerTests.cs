namespace OffndAt.Application.UnitTests.Links.Events.LinkCreated;

using Application.Links.Events.LinkCreated;
using Core.Abstractions.Messaging;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Events;
using Domain.ValueObjects;
using NSubstitute;

internal sealed class LinkCreatedDomainEventHandlerTests
{
    private LinkCreatedDomainEventHandler _handler = null!;
    private IIntegrationEventPublisher _integrationEventPublisher = null!;

    [SetUp]
    public void Setup()
    {
        _integrationEventPublisher = Substitute.For<IIntegrationEventPublisher>();

        _handler = new LinkCreatedDomainEventHandler(_integrationEventPublisher);
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

        await _handler.Handle(new LinkCreatedDomainEvent(link), CancellationToken.None);

        await _integrationEventPublisher.Received()
            .PublishAsync(Arg.Is<LinkCreatedIntegrationEvent>(e => e.LinkId == link.Id), Arg.Any<CancellationToken>());
    }
}
