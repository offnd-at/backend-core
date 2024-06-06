﻿namespace OffndAt.Application.Core.Extensions;

using Domain.Core.Primitives;
using FluentValidation;

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
    internal static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, Error error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error), "The error is required.");
        }

        return rule.WithErrorCode(error.Code).WithMessage(error.Message);
    }
}
