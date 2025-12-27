namespace OffndAt.Application.Links.Events.LinkVisited;

using Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

/// <summary>
///     Handles the LinkVisitedDomainEvent to process link visit tracking.
/// </summary>
internal sealed class LinkVisitedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<LinkVisitedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkVisitedDomainEvent notification, CancellationToken cancellationToken) =>
        await integrationEventPublisher.PublishAsync(new LinkVisitedIntegrationEvent(notification), cancellationToken);
}
