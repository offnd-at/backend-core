namespace OffndAt.Domain.Core.Events;

using MediatR;

/// <summary>
///     Defines the contract for domain events that occur within aggregate boundaries.
/// </summary>
public interface IDomainEvent : INotification;
