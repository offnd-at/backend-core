namespace OffndAt.Domain.Core.Exceptions;

using Primitives;

/// <summary>
///     Exception thrown when domain rule violations occur.
/// </summary>
public sealed class DomainException(Error error) : Exception(error.Message)
{
    /// <summary>
    ///     Gets the error.
    /// </summary>
    public Error Error { get; } = error;
}
