using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.IntegrationEvents.LinkVisited;
using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.DomainEvents.LinkVisited;

/// <summary>
///     Handles the <see cref="LinkVisitedDomainEvent" /> by publishing a <see cref="LinkVisitedIntegrationEvent" /> to external systems.
/// </summary>
/// <param name="integrationEventPublisher">The integration event publisher.</param>
internal sealed class PublishLinkVisitedIntegrationEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<LinkVisitedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkVisitedDomainEvent notification, CancellationToken cancellationToken) =>
        await integrationEventPublisher.PublishAsync(new LinkVisitedIntegrationEvent(notification), cancellationToken);
}
