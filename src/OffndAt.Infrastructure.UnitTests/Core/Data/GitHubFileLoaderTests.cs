using Microsoft.Extensions.Options;
using NSubstitute;
using Octokit;
using OffndAt.Application.Core.Constants;
using OffndAt.Infrastructure.Abstractions.Telemetry;
using OffndAt.Infrastructure.Data;
using OffndAt.Infrastructure.Data.Settings;
using Polly;
using Polly.Registry;

namespace OffndAt.Infrastructure.UnitTests.Core.Data;

internal sealed class GitHubFileLoaderTests
{
    private readonly IOptions<GitHubDataSourceSettings> _options = Options.Create(
        new GitHubDataSourceSettings
        {
            RepositoryName = "test-repo",
            RepositoryOwner = "test-owner"
        });

    private GitHubFileLoader _loader = null!;
    private IRepositoryContentsClient _repositoryContentsClient = null!;
    private ResiliencePipelineProvider<string> _resiliencePipelineProvider = null!;

    [SetUp]
    public void Setup()
    {
        var githubClient = Substitute.For<IGitHubClient>();
        var repositoriesClient = Substitute.For<IRepositoriesClient>();
        _repositoryContentsClient = Substitute.For<IRepositoryContentsClient>();
        _resiliencePipelineProvider = Substitute.For<ResiliencePipelineProvider<string>>();
        _resiliencePipelineProvider.GetPipeline(ResiliencePolicies.GitHubApiPolicyName).Returns(ResiliencePipeline.Empty);

        githubClient.Repository.Returns(repositoriesClient);
        repositoriesClient.Content.Returns(_repositoryContentsClient);

        _loader = new GitHubFileLoader(
            githubClient,
            _options,
            _resiliencePipelineProvider,
            Substitute.For<IGitHubApiUsageMetrics>());
    }

    [Test]
    public async Task DownloadAsync_ShouldCallRepositoryContentsClientWithValuesFromOptionsAndFilePath()
    {
        const string expectedPath = "path/to/file";

        await _loader.DownloadAsync(expectedPath, CancellationToken.None);

        await _repositoryContentsClient.Received(1)
            .GetRawContent(
                Arg.Is<string>(value => value == _options.Value.RepositoryOwner),
                Arg.Is<string>(value => value == _options.Value.RepositoryName),
                Arg.Is<string>(value => value == expectedPath));
    }

    [Test]
    public async Task DownloadAsync_ShouldDownloadRepositoryContents()
    {
        var expectedBytes = "string"u8.ToArray();
        _repositoryContentsClient.GetRawContent(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(expectedBytes);

        const string expectedPath = "path/to/file";

        var actual = await _loader.DownloadAsync(expectedPath, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(actual.HasValue, Is.True);
            Assert.That(actual.Value, Is.EqualTo(expectedBytes));
        });
    }
}
