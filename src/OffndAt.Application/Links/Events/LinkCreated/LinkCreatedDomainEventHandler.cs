using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Domain.Core.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.Events.LinkCreated;

/// <summary>
///     Represents the <see cref="LinkCreatedDomainEvent" /> handler.
/// </summary>
internal sealed class LinkCreatedDomainEventHandler(
    IIntegrationEventPublisher integrationEventPublisher,
    ILinkMetrics linkMetrics)
    : IDomainEventHandler<LinkCreatedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        linkMetrics.RecordLinkCreation(notification.Link.Language, notification.Link.Theme);

        await integrationEventPublisher.PublishAsync(new LinkCreatedIntegrationEvent(notification), cancellationToken);
    }
}
