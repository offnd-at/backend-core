namespace OffndAt.Domain.Core.Primitives;

/// <summary>
///     Represents a domain error.
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

    public static implicit operator string(Error? error) => error?.Code ?? string.Empty;

    /// <inheritdoc />
    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
        yield return Message;
    }
}
