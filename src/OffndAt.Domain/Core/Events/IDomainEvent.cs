using MediatR;


namespace OffndAt.Domain.Core.Events;/// <summary>
///     Defines the contract for domain events that occur within aggregate boundaries.
/// </summary>
public interface IDomainEvent : INotification;
