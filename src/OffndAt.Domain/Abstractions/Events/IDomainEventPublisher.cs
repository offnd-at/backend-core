namespace OffndAt.Domain.Abstractions.Events;

/// <summary>
///     Represents the domain event publisher interface.
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    ///     Publishes the specified domain event to the appropriate event handlers.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TDomainEvent">The domain event type.</typeparam>
    /// <returns>The completed task.</returns>
    Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
        where TDomainEvent : IDomainEvent;
}
