using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.IntegrationEvents.LinkCreated;
using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.DomainEvents.LinkCreated;

/// <summary>
///     Handles the <see cref="LinkCreatedDomainEvent" /> by publishing a <see cref="LinkCreatedIntegrationEvent" /> to external systems.
/// </summary>
/// <param name="integrationEventPublisher">The integration event publisher.</param>
internal sealed class PublishLinkCreatedIntegrationEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<LinkCreatedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await integrationEventPublisher.PublishAsync(new LinkCreatedIntegrationEvent(notification), cancellationToken);
}
