namespace OffndAt.Domain.ValueObjects;

using Core.Errors;
using Core.Extensions;
using Core.Primitives;

/// <summary>
///     Represents the URL value object.
/// </summary>
public sealed class Url : ValueObject
{
    /// <summary>
    ///     The maximum length.
    /// </summary>
    public const int MaxLength = 4096;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Url" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    private Url(string value) => Value = value;

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public string Value { get; }

    public static implicit operator string(Url url) => url.Value;

    /// <summary>
    ///     Creates a new <see cref="Url" /> instance based on the specified value.
    /// </summary>
    /// <param name="url">The value.</param>
    /// <returns>The result of the value object creation process containing the <see cref="Url" /> or an <see cref="Error" />.</returns>
    public static Result<Url> Create(string url) =>
        Result.Create(url, DomainErrors.Url.NullOrEmpty)
            .Ensure(value => !string.IsNullOrWhiteSpace(value), DomainErrors.Url.NullOrEmpty)
            .Ensure(value => value.Length <= MaxLength, DomainErrors.Url.LongerThanAllowed)
            .Map(value => new Url(value));

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
