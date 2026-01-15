namespace OffndAt.Domain.Abstractions.Events;

/// <summary>
///     Collects domain events raised during a transaction that are not attached to any entity.
/// </summary>
public interface IDomainEventCollector
{
    /// <summary>
    ///     Raises a domain event and adds it to the collector.
    /// </summary>
    /// <param name="domainEvent">The domain event to be collected.</param>
    void RaiseEvent(IDomainEvent domainEvent);

    /// <summary>
    ///     Retrieves all collected domain events.
    /// </summary>
    /// <returns>A read-only list of domain events that were collected.</returns>
    IReadOnlyList<IDomainEvent> GetEvents();

    /// <summary>
    ///     Clears all collected domain events from the collector.
    /// </summary>
    void ClearEvents();
}
