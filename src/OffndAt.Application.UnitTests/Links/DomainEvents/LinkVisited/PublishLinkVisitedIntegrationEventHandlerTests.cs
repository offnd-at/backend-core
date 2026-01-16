using NSubstitute;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.DomainEvents.LinkVisited;
using OffndAt.Application.Links.IntegrationEvents.LinkVisited;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.DomainEvents.LinkVisited;

internal sealed class PublishLinkVisitedIntegrationEventHandlerTests
{
    private PublishLinkVisitedIntegrationEventHandler _handler = null!;
    private IIntegrationEventPublisher _integrationEventPublisher = null!;

    [SetUp]
    public void Setup()
    {
        _integrationEventPublisher = Substitute.For<IIntegrationEventPublisher>();

        _handler = new PublishLinkVisitedIntegrationEventHandler(_integrationEventPublisher);
    }

    [Test]
    public async Task Handle_ShouldPublishIntegrationEvent()
    {
        var linkId = new LinkId(Guid.NewGuid());
        var context = new LinkVisitedContext(Language.English, Theme.None, DateTimeOffset.Now);

        await _handler.Handle(new LinkVisitedDomainEvent(linkId, context), TestContext.CurrentContext.CancellationToken);

        await _integrationEventPublisher.Received()
            .PublishAsync(
                Arg.Is<LinkVisitedIntegrationEvent>(e =>
                    e.LinkId == linkId &&
                    e.LanguageId == context.Language.Value &&
                    e.ThemeId == context.Theme.Value),
                Arg.Any<CancellationToken>());
    }
}
