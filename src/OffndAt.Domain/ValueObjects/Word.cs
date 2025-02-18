namespace OffndAt.Domain.ValueObjects;

using Core.Errors;
using Core.Extensions;
using Core.Primitives;

/// <summary>
///     Represents the word value object.
/// </summary>
public sealed class Word : ValueObject
{
    /// <summary>
    ///     The maximum length.
    /// </summary>
    public const int MaxLength = 256;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Word" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    private Word(string value) => Value = value;

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Implicitly converts a <see cref="Word" /> to a <see cref="string" /> by returning its value.
    /// </summary>
    /// <param name="word">The <see cref="Word" /> instance to convert.</param>
    /// <returns>A <see cref="string" /> representing the value of the <paramref name="word" />.</returns>
    public static implicit operator string(Word word) => word.Value;

    /// <summary>
    ///     Creates a new <see cref="Word" /> instance based on the specified value.
    /// </summary>
    /// <param name="word">The value.</param>
    /// <returns>The result of the value object creation process containing the <see cref="Word" /> or an <see cref="Error" />.</returns>
    public static Result<Word> Create(string word) =>
        Result.Create(word, DomainErrors.Word.NullOrEmpty)
            .Ensure(value => !string.IsNullOrWhiteSpace(value), DomainErrors.Word.NullOrEmpty)
            .Ensure(value => value.Length <= MaxLength, DomainErrors.Word.LongerThanAllowed)
            .Map(value => new Word(value));

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
