namespace OffndAt.Services.Api.Middleware.Extensions;

/// <summary>
///     Contains extension methods for configuring the exception handler middleware.
/// </summary>
internal static class ExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    ///     Configures the custom exception handler middleware.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The configured application builder.</returns>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ExceptionHandlerMiddleware>();
}
