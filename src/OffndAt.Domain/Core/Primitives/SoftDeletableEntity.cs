namespace OffndAt.Domain.Core.Primitives;

using Abstractions;

/// <summary>
///     Represent a base soft deletable entity.
/// </summary>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class SoftDeletableEntity<TEntityId> : Entity<TEntityId>, ISoftDeletableEntity where TEntityId : EntityId
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SoftDeletableEntity{TEntityId}" /> class.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected SoftDeletableEntity(TEntityId id)
        : base(id)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SoftDeletableEntity{TEntityId}" /> class.
    /// </summary>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    protected SoftDeletableEntity()
    {
    }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; }

    /// <inheritdoc />
    public bool IsDeleted { get; }
}
