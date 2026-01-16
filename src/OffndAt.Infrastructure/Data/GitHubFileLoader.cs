using System.Diagnostics;
using Microsoft.Extensions.Options;
using Octokit;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Core.Constants;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Infrastructure.Abstractions.Telemetry;
using OffndAt.Infrastructure.Data.Settings;
using Polly.Registry;

namespace OffndAt.Infrastructure.Data;

/// <summary>
///     Represents the GitHub file loader.
/// </summary>
/// <param name="githubClient">The GitHub client.</param>
/// <param name="githubOptions">The GitHub options.</param>
/// <param name="resiliencePipelineProvider">The provider for Polly resilience pipelines.</param>
/// <param name="metrics">The GitHub API usage metrics.</param>
internal sealed class GitHubFileLoader(
    IGitHubClient githubClient,
    IOptions<GitHubDataSourceSettings> githubOptions,
    ResiliencePipelineProvider<string> resiliencePipelineProvider,
    IGitHubApiUsageMetrics metrics)
    : IFileLoader
{
    private readonly GitHubDataSourceSettings _githubSettings = githubOptions.Value;

    /// <inheritdoc />
    public async Task<Maybe<byte[]>> DownloadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var githubApiPipeline = resiliencePipelineProvider.GetPipeline(ResiliencePolicies.GitHubApiPolicyName);

        try
        {
            metrics.RecordRequest();

            var result = await githubApiPipeline.ExecuteAsync<byte[]>(
                async _ => await githubClient.Repository.Content.GetRawContent(
                    _githubSettings.RepositoryOwner,
                    _githubSettings.RepositoryName,
                    filePath),
                cancellationToken);

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
