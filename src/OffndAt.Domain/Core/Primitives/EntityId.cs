namespace OffndAt.Domain.Core.Primitives;

using Utils;

/// <summary>
///     Represents the entity identifier value object.
/// </summary>
public abstract class EntityId : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityId" /> class.
    /// </summary>
    /// <param name="value">The entity identifier value.</param>
    protected EntityId(Guid value)
    {
        Ensure.NotEmpty(value, "The entity identifier is required.", nameof(value));

        Value = value;
    }

    /// <summary>
    ///     Gets the entity identifier value.
    /// </summary>
    public Guid Value { get; }

    public static implicit operator string(EntityId entityId) => entityId.Value.ToString();

    public static implicit operator Guid(EntityId entityId) => entityId.Value;

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
