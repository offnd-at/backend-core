using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.Core.Exceptions;

/// <summary>
///     Represents the exception that occurs when an invariant is violated.
/// </summary>
public sealed class InvariantViolationException(Error? error = null)
    : DomainException(error ?? DomainErrors.General.UnprocessableRequest);
