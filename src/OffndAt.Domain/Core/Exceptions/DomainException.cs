using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.Core.Exceptions;

/// <summary>
///     Represents an exception that occurred in the domain.
/// </summary>
public sealed class DomainException(Error error) : Exception(error.Message)
{
    /// <summary>
    ///     Gets the error.
    /// </summary>
    public Error Error { get; } = error;
}
