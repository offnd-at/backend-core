namespace OffndAt.Domain.Core.Primitives;

using Utils;

/// <summary>
///     Represents a base entity that all other entities derive from.
/// </summary>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class Entity<TEntityId> : IEquatable<Entity<TEntityId>> where TEntityId : EntityId
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity{TEntityId}" /> class.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    protected Entity(TEntityId id)
        : this()
    {
        Ensure.NotNull(id, "The identifier is required.", nameof(id));

        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Entity{TEntityId}" /> class.
    /// </summary>
    /// <remarks>
    ///     Required by EF Core.
    /// </remarks>
    protected Entity()
    {
    }

    /// <summary>
    ///     Gets or sets the entity identifier.
    /// </summary>
    public TEntityId Id { get; }

    /// <inheritdoc />
    public bool Equals(Entity<TEntityId>? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || Id == other.Id;
    }

    public static bool operator ==(Entity<TEntityId>? a, Entity<TEntityId>? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TEntityId> a, Entity<TEntityId> b) => !(a == b);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity<TEntityId> other)
        {
            return false;
        }

        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode() * 41;
}
