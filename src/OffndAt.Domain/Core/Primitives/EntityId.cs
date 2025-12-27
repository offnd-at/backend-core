using Utils;


namespace OffndAt.Domain.Core.Primitives;/// <summary>
///     Provides strongly-typed entity identifier with validation.
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

    /// <summary>
    ///     Implicitly converts an <see cref="EntityId" /> to a <see cref="string" /> by returning the string representation of its value.
    /// </summary>
    /// <param name="entityId">The <see cref="EntityId" /> instance to convert.</param>
    /// <returns>A <see cref="string" /> representing the value of the <paramref name="entityId" />.</returns>
    public static implicit operator string(EntityId entityId) => entityId.Value.ToString();

    /// <summary>
    ///     Implicitly converts an <see cref="EntityId" /> to a <see cref="Guid" /> by returning its value.
    /// </summary>
    /// <param name="entityId">The <see cref="EntityId" /> instance to convert.</param>
    /// <returns>A <see cref="Guid" /> representing the value of the <paramref name="entityId" />.</returns>
    public static implicit operator Guid(EntityId entityId) => entityId.Value;

    /// <inheritdoc />
    public override string ToString() => Value.ToString();

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
