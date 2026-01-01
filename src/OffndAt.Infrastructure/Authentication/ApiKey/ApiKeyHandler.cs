using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OffndAt.Domain.Core.Errors;
using OffndAt.Infrastructure.Authentication.Settings;

namespace OffndAt.Infrastructure.Authentication.ApiKey;

/// <summary>
///     Represents the API key authentication handler.
/// </summary>
/// <param name="options">The monitor for the options instance.</param>
/// <param name="logger">The logger factory.</param>
/// <param name="encoder">The URL encoder.</param>
/// <param name="authOptions">The authentication options.</param>
internal sealed class ApiKeyHandler(
    IOptionsMonitor<ApiKeyOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<AuthSettings> authOptions)
    : AuthenticationHandler<ApiKeyOptions>(options, logger, encoder)
{
    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValue) || string.IsNullOrEmpty(headerValue.FirstOrDefault()))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }

        var inputBytes = Encoding.UTF8.GetBytes(headerValue.FirstOrDefault()!);
        var storedBytes = Encoding.UTF8.GetBytes(authOptions.Value.ApiKey);

        if (inputBytes.Length != storedBytes.Length || !CryptographicOperations.FixedTimeEquals(inputBytes, storedBytes))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Worker")
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <inheritdoc />
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var problemDetailsService = Context.RequestServices.GetRequiredService<IProblemDetailsService>();

        Context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = Context,
                ProblemDetails = new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Extensions = new Dictionary<string, object?>
                    {
                        {
                            "errors", new[]
                            {
                                DomainErrors.General.Unauthorized
                            }
                        }
                    }
                }
            });
    }
}
