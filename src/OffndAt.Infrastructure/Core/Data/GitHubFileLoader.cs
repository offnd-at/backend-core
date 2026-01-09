using System.Diagnostics;
using Microsoft.Extensions.Options;
using Octokit;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Infrastructure.Core.Abstractions.Telemetry;
using OffndAt.Infrastructure.Core.Data.Settings;

namespace OffndAt.Infrastructure.Core.Data;

/// <summary>
///     Represents the GitHub file loader.
/// </summary>
/// <param name="githubClient">The GitHub client.</param>
/// <param name="githubOptions">The GitHub options.</param>
/// <param name="metrics">The GitHub API usage metrics.</param>
internal sealed class GitHubFileLoader(
    IGitHubClient githubClient,
    IOptions<GitHubDataSourceSettings> githubOptions,
    IGitHubApiUsageMetrics metrics)
    : IFileLoader
{
    private readonly GitHubDataSourceSettings _githubSettings = githubOptions.Value;

    /// <inheritdoc />
    public async Task<Maybe<byte[]>> DownloadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            metrics.RecordRequest();

            var result = await githubClient.Repository.Content.GetRawContent(
                _githubSettings.RepositoryOwner,
                _githubSettings.RepositoryName,
                filePath);

            var apiInfo = githubClient.GetLastApiInfo();
            if (apiInfo?.RateLimit?.Remaining is not null)
            {
                metrics.RecordRemainingRateLimit(apiInfo.RateLimit.Remaining);
            }

            return result;
        }
        finally
        {
            stopwatch.Stop();
            metrics.RecordRequestLatency(stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
