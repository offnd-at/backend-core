namespace OffndAt.Domain.Core.Primitives;

/// <summary>
///     Encapsulates error information including code and message for domain operations.
/// </summary>
public sealed class Error(string code, string message) : ValueObject
{
    /// <summary>
    ///     Gets the error code.
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    ///     Gets the error message.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    ///     Gets the empty error instance.
    /// </summary>
    public static Error None => new(string.Empty, string.Empty);

    /// <summary>
    ///     Implicitly converts an <see cref="Error" /> to a <see cref="string" /> by returning the <see cref="Error.Code" />.
    /// </summary>
    /// <param name="error">The <see cref="Error" /> instance to convert.</param>
    /// <returns>The <see cref="Error.Code" /> if the <paramref name="error" /> is not null; otherwise, an empty string.</returns>
    public static implicit operator string(Error? error) => error?.Code ?? string.Empty;

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return Message;
    }
}
