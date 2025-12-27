using Events;


namespace OffndAt.Domain.Core.Abstractions;/// <summary>
///     Marks entities as aggregate roots in Domain-Driven Design.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    ///     Gets the domain events. This collection is readonly.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    ///     Clears all the domain events from the <see cref="IAggregateRoot" />.
    /// </summary>
    void ClearDomainEvents();
}
