using OffndAt.Domain.Core.Events;

namespace OffndAt.Domain.Abstractions;

/// <summary>
///     Represents an aggregate root interface.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    ///     Gets the domain events. This collection is readonly.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    ///     Clears all the domain events from the <see cref="IAggregateRoot" />.
    /// </summary>
    public void ClearDomainEvents();
}
