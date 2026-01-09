using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Application.Core.Abstractions.Telemetry;
using OffndAt.Domain.Core.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.Events.LinkVisited;

/// <summary>
///     Represents the <see cref="LinkVisitedDomainEvent" /> handler.
/// </summary>
internal sealed class LinkVisitedDomainEventHandler(
    IIntegrationEventPublisher integrationEventPublisher,
    IVisitMetrics visitMetrics)
    : IDomainEventHandler<LinkVisitedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkVisitedDomainEvent notification, CancellationToken cancellationToken)
    {
        visitMetrics.RecordVisit(notification.Link.Language, notification.Link.Theme);

        await integrationEventPublisher.PublishAsync(new LinkVisitedIntegrationEvent(notification), cancellationToken);
    }
}
