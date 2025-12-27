using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Domain.Core.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.Events.LinkCreated;

/// <summary>
///     Handles the LinkCreatedDomainEvent to process new link creation.
/// </summary>
internal sealed class LinkCreatedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<LinkCreatedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await integrationEventPublisher.PublishAsync(new LinkCreatedIntegrationEvent(notification), cancellationToken);
}
