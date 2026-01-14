using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Domain.Core.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.Events.LinkVisited;

/// <summary>
///     Represents the <see cref="LinkVisitedDomainEvent" /> handler.
/// </summary>
/// <param name="integrationEventPublisher">The integration event publisher.</param>
/// <param name="linkMetrics">The collection of link-related metrics.</param>
internal sealed class LinkVisitedDomainEventHandler(
    IIntegrationEventPublisher integrationEventPublisher,
    ILinkMetrics linkMetrics)
    : IDomainEventHandler<LinkVisitedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkVisitedDomainEvent notification, CancellationToken cancellationToken)
    {
        linkMetrics.RecordLinkVisit(notification.Link.Language, notification.Link.Theme);

        await integrationEventPublisher.PublishAsync(new LinkVisitedIntegrationEvent(notification), cancellationToken);
    }
}
