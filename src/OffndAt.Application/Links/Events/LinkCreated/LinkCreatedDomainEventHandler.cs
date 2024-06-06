namespace OffndAt.Application.Links.Events.LinkCreated;

using Core.Abstractions.Messaging;
using Domain.Core.Events;
using Domain.Events;

/// <summary>
///     Represents the <see cref="LinkCreatedDomainEvent" /> handler.
/// </summary>
internal sealed class LinkCreatedDomainEventHandler(IIntegrationEventPublisher integrationEventPublisher)
    : IDomainEventHandler<LinkCreatedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkCreatedDomainEvent notification, CancellationToken cancellationToken) =>
        await integrationEventPublisher.PublishAsync(new LinkCreatedIntegrationEvent(notification), cancellationToken);
}
