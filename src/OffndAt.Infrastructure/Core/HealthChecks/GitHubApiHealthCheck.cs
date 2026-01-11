using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OffndAt.Infrastructure.Core.HealthChecks;

/// <summary>
///     Represents a health check for validating the availability of the GitHub API.
/// </summary>
/// <param name="httpClient">The HTTP client.</param>
internal sealed class GitHubApiHealthCheck(HttpClient httpClient) : IHealthCheck
{
    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonResponse = await httpClient.GetStringAsync("https://www.githubstatus.com/api/v2/status.json", cancellationToken);
            using var jsonDoc = JsonDocument.Parse(jsonResponse);

            if (!jsonDoc.RootElement.TryGetProperty("status", out var statusElement) ||
                !statusElement.TryGetProperty("indicator", out var indicatorElement))
            {
                return HealthCheckResult.Unhealthy("GitHub API is unavailable.");
            }

            var statusIndicator = indicatorElement.GetString();
            return statusIndicator switch
            {
                "none" => HealthCheckResult.Healthy(),
                "minor" => HealthCheckResult.Degraded("GitHub API is experiencing minor outage."),
                "major" => HealthCheckResult.Degraded("GitHub API is experiencing major outage."),
                "critical" => HealthCheckResult.Unhealthy("GitHub API is experiencing critical outage."),
                _ => HealthCheckResult.Unhealthy("GitHub API is unavailable.")
            };
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("GitHub API health check timed out.", ex);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("GitHub API health check failed.", ex);
        }
    }
}
