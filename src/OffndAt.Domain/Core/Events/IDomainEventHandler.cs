namespace OffndAt.Domain.Core.Events;

using MediatR;

/// <summary>
///     Defines the contract for handling domain events.
/// </summary>
/// <typeparam name="TDomainEvent">The domain event type.</typeparam>
public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;
