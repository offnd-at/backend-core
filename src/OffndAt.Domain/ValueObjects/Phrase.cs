using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.ValueObjects;

/// <summary>
///     Represents the phrase value object.
/// </summary>
public sealed class Phrase : ValueObject
{
    /// <summary>
    ///     The maximum length.
    /// </summary>
    public const int MaxLength = 256;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Phrase" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    private Phrase(string value) => Value = value;

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => $"{nameof(Phrase)} {{ Value = {Value} }}";

    /// <summary>
    ///     Implicitly converts a <see cref="Phrase" /> to a <see cref="string" /> by returning its value.
    /// </summary>
    /// <param name="phrase">The <see cref="Phrase" /> instance to convert.</param>
    /// <returns>A <see cref="string" /> representing the value of the <paramref name="phrase" />.</returns>
    public static implicit operator string(Phrase phrase) => phrase.Value;

    /// <summary>
    ///     Creates a new <see cref="Phrase" /> instance based on the specified value.
    /// </summary>
    /// <param name="phrase">The value.</param>
    /// <returns>The result of the value object creation process containing the <see cref="Phrase" /> or an <see cref="Error" />.</returns>
    public static Result<Phrase> Create(string phrase) =>
        Result.Create(phrase, DomainErrors.Phrase.NullOrEmpty)
            .Ensure(value => !string.IsNullOrWhiteSpace(value), DomainErrors.Phrase.NullOrEmpty)
            .Ensure(value => value.Length <= MaxLength, DomainErrors.Phrase.LongerThanAllowed)
            .Map(value => new Phrase(value));

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
