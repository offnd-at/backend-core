using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Exceptions;

namespace OffndAt.Domain.Core.Guards;

/// <summary>
///     Contains assertions for the domain objects' invariants.
/// </summary>
public static class Guard
{
    /// <summary>
    ///     Ensures that the specified string value is not null, empty, or whitespace-only.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="errorMessage">The message to include in the domain error.</param>
    /// <exception cref="InvariantViolationException">when the specified value is null, empty, or whitespace.</exception>
    public static void AgainstEmpty(string? value, string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvariantViolationException(DomainErrors.General.InvariantViolated(errorMessage));
        }
    }

    /// <summary>
    ///     Ensures that the specified Guid value is not null or empty.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="errorMessage">The message to include in the domain error.</param>
    /// <exception cref="InvariantViolationException">when the specified value is null or empty.</exception>
    public static void AgainstEmpty(Guid? value, string errorMessage)
    {
        if (value is null || value == Guid.Empty)
        {
            throw new InvariantViolationException(DomainErrors.General.InvariantViolated(errorMessage));
        }
    }

    /// <summary>
    ///     Ensures that the specified collection is not null or empty.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="errorMessage">The message to include in the domain error.</param>
    /// <exception cref="InvariantViolationException">when the specified collection is null or empty.</exception>
    public static void AgainstEmpty<T>(IEnumerable<T>? value, string errorMessage)
    {
        if (value is null || !value.Any())
        {
            throw new InvariantViolationException(DomainErrors.General.InvariantViolated(errorMessage));
        }
    }

    /// <summary>
    ///     Ensures that the specified value is not null.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="errorMessage">The message to include in the domain error.</param>
    /// <exception cref="InvariantViolationException">when the specified collection is null.</exception>
    public static void AgainstNull<T>(T? value, string errorMessage) where T : class
    {
        if (value is null)
        {
            throw new InvariantViolationException(DomainErrors.General.InvariantViolated(errorMessage));
        }
    }
}
