namespace OffndAt.Domain.Core.Primitives;

/// <summary>
///     Represents a domain error.
/// </summary>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    ///     Gets the empty error instance.
    /// </summary>
    public static Error None =>
        new(string.Empty, string.Empty);

    /// <inheritdoc />
    public override string ToString() => $"{{ Code = {Code}, Message = {Message} }}";

    /// <summary>
    ///     Implicitly converts an <see cref="Error" /> to a <see cref="string" /> by returning the <see cref="Error.Code" />.
    /// </summary>
    /// <param name="error">The <see cref="Error" /> instance to convert.</param>
    /// <returns>The <see cref="Error.Code" /> if the <paramref name="error" /> is not null; otherwise, an empty string.</returns>
    public static implicit operator string(Error? error) => error?.Code ?? string.Empty;
}
