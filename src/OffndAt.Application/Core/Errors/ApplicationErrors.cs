using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Errors;

/// <summary>
///     Contains application errors.
/// </summary>
public sealed class ApplicationErrors
{
    /// <summary>
    ///     Gets the too many requests error.
    /// </summary>
    public static Error TooManyRequests =>
        new("General.TooManyRequests", "Too many requests. Please try again later.");
}
