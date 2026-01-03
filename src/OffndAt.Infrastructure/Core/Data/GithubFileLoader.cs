using Microsoft.Extensions.Options;
using Octokit;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Infrastructure.Core.Data.Settings;

namespace OffndAt.Infrastructure.Core.Data;

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
