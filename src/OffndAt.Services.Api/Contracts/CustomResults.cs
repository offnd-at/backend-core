using Domain.Core.Errors;
using Domain.Core.Primitives;
using Microsoft.AspNetCore.Mvc;


namespace OffndAt.Services.Api.Contracts;/// <summary>
///     Contains the API custom results.
/// </summary>
internal static class CustomResults
{
    /// <summary>
    ///     Produces a <see cref="StatusCodes.Status400BadRequest" /> response in accordance with <see cref="ProblemDetails" /> specification.
    /// </summary>
    /// <param name="error">An error object to be included in the HTTP response body.</param>
    /// <returns>The created <see cref="IResult" /> for the response.</returns>
    internal static IResult BadRequest(Error error) =>
        TypedResults.Problem(
            error.Message,
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { { "errors", new[] { error } } });

    /// <summary>
    ///     Produces a <see cref="StatusCodes.Status404NotFound" /> response in accordance with <see cref="ProblemDetails" /> specification.
    /// </summary>
    /// <param name="error">An error object to be included in the HTTP response body.</param>
    /// <returns>The created <see cref="IResult" /> for the response.</returns>
    internal static IResult NotFound(Error? error = null)
    {
        error ??= DomainErrors.General.NotFound;

        return TypedResults.Problem(
            error.Message,
            statusCode: StatusCodes.Status404NotFound,
            extensions: new Dictionary<string, object?> { { "errors", new[] { error } } });
    }
}
