using Microsoft.AspNetCore.Authentication;
using OffndAt.Infrastructure.Core.Constants;

namespace OffndAt.Infrastructure.Authentication.ApiKey;

/// <summary>
///     Contains the options used by the <see cref="ApiKeyHandler" />.
/// </summary>
public sealed class ApiKeyOptions : AuthenticationSchemeOptions
{
    /// <summary>
    ///     Represents the name of the header used to read the API key from.
    /// </summary>
    public string HeaderName { get; init; } = HeaderNames.ApiKey;
}
