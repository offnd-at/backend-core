using OffndAt.Domain.Core.Abstractions;
using OffndAt.Domain.Core.Events;

namespace OffndAt.Domain.Core.Primitives;

/// <summary>
///     Represents an aggregate root.
/// </summary>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class AggregateRoot<TEntityId> : SoftDeletableEntity<TEntityId>, IAggregateRoot where TEntityId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="AggregateRoot{TEntityId}" /> class.
    /// </summary>
    /// <param name="id">The aggregate root identifier.</param>
    protected AggregateRoot(TEntityId id)
        : base(id)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AggregateRoot{TEntityId}" /> class.
    /// </summary>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    protected AggregateRoot()
    {
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    ///     Raises the specified <see cref="IDomainEvent" /> in the context of the <see cref="AggregateRoot{TEntityId}" />.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    public void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
