using System.Net;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Application.Core.Exceptions;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Exceptions;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Services.Api.Middleware;

/// <summary>
///     Provides centralized exception handling and problem details responses for the API.
/// </summary>
/// <param name="next">The next middleware delegate.</param>
/// <param name="problemDetailsService">The problem details service.</param>
/// <param name="logger">The logger.</param>
internal sealed class ExceptionHandlerMiddleware(
    RequestDelegate next,
    IProblemDetailsService problemDetailsService,
    ILogger<ExceptionHandlerMiddleware> logger)
{
    /// <summary>
    ///     Invokes the middleware with the specified <see cref="HttpContext" />.
    /// </summary>
    /// <param name="httpContext">The HTTP httpContext.</param>
    /// <returns>The task that can be awaited by the next middleware.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    ///     Handles the specified <see cref="Exception" /> for the specified <see cref="HttpContext" />.
    /// </summary>
    /// <param name="httpContext">The HTTP httpContext.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The HTTP response that is modified based on the exception.</returns>
    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var (httpStatusCode, title, errors) = GetHttpStatusCodeAndErrors(exception);

        httpContext.Response.StatusCode = httpStatusCode;

        _ = await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Title = title,
                    Status = httpStatusCode,
                    Extensions = new Dictionary<string, object?> { { "errors", errors } }
                }
            });
    }

    /// <summary>
    ///     Maps the specified exception to HTTP status code and errors.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The <see cref="HttpStatusCode" /> instance alongside with a list of errors.</returns>
    private static (int httpStatusCode, string title, IReadOnlyCollection<Error>) GetHttpStatusCodeAndErrors(Exception exception) =>
        exception switch
        {
            ValidationException validationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred.",
                validationException.Errors),
            DomainException domainException => (StatusCodes.Status400BadRequest, "An application error occurred.", [domainException.Error]),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", [DomainErrors.General.ServerError])
        };
}
