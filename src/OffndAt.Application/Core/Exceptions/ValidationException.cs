namespace OffndAt.Application.Core.Exceptions;

using Domain.Core.Primitives;
using FluentValidation.Results;

/// <summary>
///     Represents an exception that occurs when a validation fails.
/// </summary>
public sealed class ValidationException(IEnumerable<ValidationFailure> failures)
    : Exception("One or more validation failures has occurred.")
{
    /// <summary>
    ///     Gets the validation errors.
    /// </summary>
    public IReadOnlyCollection<Error> Errors { get; } = failures
        .Distinct()
        .Select(failure => new Error(failure.ErrorCode, failure.ErrorMessage))
        .ToList();
}
