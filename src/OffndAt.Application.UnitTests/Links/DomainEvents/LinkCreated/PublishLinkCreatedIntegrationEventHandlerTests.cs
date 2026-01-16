using NSubstitute;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.DomainEvents.LinkCreated;
using OffndAt.Application.Links.IntegrationEvents.LinkCreated;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.DomainEvents.LinkCreated;

internal sealed class PublishLinkCreatedIntegrationEventHandlerTests
{
    private PublishLinkCreatedIntegrationEventHandler _handler = null!;
    private IIntegrationEventPublisher _integrationEventPublisher = null!;

    [SetUp]
    public void Setup()
    {
        _integrationEventPublisher = Substitute.For<IIntegrationEventPublisher>();

        _handler = new PublishLinkCreatedIntegrationEventHandler(_integrationEventPublisher);
    }

    [Test]
    public async Task Handle_ShouldPublishIntegrationEvent()
    {
        var linkId = new LinkId(Guid.NewGuid());
        var language = Language.English;
        var theme = Theme.None;

        await _handler.Handle(new LinkCreatedDomainEvent(linkId, language, theme), CancellationToken.None);

        await _integrationEventPublisher.Received()
            .PublishAsync(
                Arg.Is<LinkCreatedIntegrationEvent>(e =>
                    e.LinkId == linkId &&
                    e.LanguageId == language.Value &&
                    e.ThemeId == theme.Value),
                Arg.Any<CancellationToken>());
    }
}
