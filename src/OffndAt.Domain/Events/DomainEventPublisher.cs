using MediatR;
using OffndAt.Domain.Abstractions.Events;

namespace OffndAt.Domain.Events;

/// <summary>
///     Represents an implementation of the <see cref="IDomainEventPublisher" /> interface,
///     providing functionality to publish domain events to appropriate handlers.
/// </summary>
/// <param name="mediator">The mediator.</param>
internal sealed class DomainEventPublisher(IMediator mediator) : IDomainEventPublisher
{
    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) where TEvent : IDomainEvent =>
        mediator.Publish(domainEvent, cancellationToken);
}
