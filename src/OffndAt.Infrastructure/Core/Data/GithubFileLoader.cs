namespace OffndAt.Infrastructure.Core.Data;

using Application.Core.Abstractions.Data;
using Domain.Core.Primitives;
using Microsoft.Extensions.Options;
using Octokit;
using Settings;

/// <summary>
///     Represents the GitHub file loader.
/// </summary>
/// <param name="githubClient">The GitHub client.</param>
/// <param name="githubOptions">The GitHub options.</param>
internal sealed class GithubFileLoader(IGitHubClient githubClient, IOptions<GithubDataSourceSettings> githubOptions)
    : IFileLoader
{
    private readonly GithubDataSourceSettings _githubSettings = githubOptions.Value;

    /// <inheritdoc />
    public async Task<Maybe<byte[]>> DownloadAsync(string filePath, CancellationToken cancellationToken = default) =>
        await githubClient.Repository.Content.GetRawContent(_githubSettings.RepositoryOwner, _githubSettings.RepositoryName, filePath);
}
