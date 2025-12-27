using FluentValidation;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Extensions;

/// <summary>
///     Contains extension methods for fluent validations.
/// </summary>
internal static class FluentValidationExtensions
{
    /// <summary>
    ///     Specifies a custom error to use if validation fails.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <typeparam name="TProperty">The property being validated.</typeparam>
    /// <param name="rule">The current rule.</param>
    /// <param name="error">The error to use.</param>
    /// <returns>The same rule builder.</returns>
    internal static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Error error) =>
        error is null
            ? throw new ArgumentNullException(nameof(error), "The error is required.")
            : rule.WithErrorCode(error.Code).WithMessage(error.Message);
}
