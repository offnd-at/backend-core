namespace OffndAt.Services.Api.Middleware;

using System.Net;
using System.Text.Json;
using Application.Core.Exceptions;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Exceptions;
using Domain.Core.Primitives;

/// <summary>
///     Represents the exception handler middleware.
/// </summary>
internal sealed class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>
    ///     Invokes the exception handler middleware with the specified <see cref="HttpContext" />.
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
            logger.LogError(ex, "An exception occurred: {Message}", ex.Message);

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    ///     Handles the specified <see cref="Exception" /> for the specified <see cref="HttpContext" />.
    /// </summary>
    /// <param name="httpContext">The HTTP httpContext.</param>
    /// <param name="exception">The exception.</param>
    /// <returns>The HTTP response that is modified based on the exception.</returns>
    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var (httpStatusCode, errors) = GetHttpStatusCodeAndErrors(exception);

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)httpStatusCode;

        var response = JsonSerializer.Serialize(new ApiErrorResponse(errors), JsonSerializerOptions);

        await httpContext.Response.WriteAsync(response);
    }

    /// <summary>
    ///     Maps the specified exception to HTTP status code and errors.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The <see cref="HttpStatusCode" /> instance alongside with a list of errors.</returns>
    private static (HttpStatusCode httpStatusCode, IReadOnlyCollection<Error>) GetHttpStatusCodeAndErrors(Exception exception) =>
        exception switch
        {
            ValidationException validationException => (HttpStatusCode.BadRequest, validationException.Errors),
            DomainException domainException => (HttpStatusCode.BadRequest, [domainException.Error]),
            _ => (HttpStatusCode.InternalServerError, new[] { DomainErrors.General.ServerError })
        };
}
