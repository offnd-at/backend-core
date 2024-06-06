namespace OffndAt.Services.Api.Contracts;

using Domain.Core.Primitives;

/// <summary>
///     Represents an API error response.
/// </summary>
internal sealed class ApiErrorResponse(IEnumerable<Error> errors)
{
    /// <summary>
    ///     Gets the errors.
    /// </summary>
    public IEnumerable<Error> Errors { get; } = errors;
}
