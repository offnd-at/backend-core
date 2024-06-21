namespace OffndAt.Application.Links.Events.LinkVisited;

using Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

/// <summary>
///     Represents the <see cref="LinkVisitedDomainEvent" /> handler.
/// </summary>
internal sealed class LinkVisitedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<LinkVisitedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkVisitedDomainEvent notification, CancellationToken cancellationToken) =>
        await integrationEventPublisher.PublishAsync(new LinkVisitedIntegrationEvent(notification), cancellationToken);
}
