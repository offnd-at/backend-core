using OffndAt.Domain.Abstractions.Events;

namespace OffndAt.Domain.Events;

/// <summary>
///     Provides functionality to collect domain events raised during a transaction that are not attached to any entity.
/// </summary>
internal sealed class DomainEventCollector : IDomainEventCollector
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <inheritdoc />
    public void RaiseEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <inheritdoc />
    public IReadOnlyList<IDomainEvent> GetEvents() => _domainEvents.ToList().AsReadOnly();

    /// <inheritdoc />
    public void ClearEvents() => _domainEvents.Clear();
}
