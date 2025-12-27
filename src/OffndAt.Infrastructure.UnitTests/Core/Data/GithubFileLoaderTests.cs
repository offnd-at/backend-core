using Infrastructure.Core.Data;
using Infrastructure.Core.Data.Settings;
using Microsoft.Extensions.Options;
using NSubstitute;
using Octokit;


namespace OffndAt.Infrastructure.UnitTests.Core.Data;internal sealed class GithubFileLoaderTests
{
    private readonly IOptions<GithubDataSourceSettings> _options = Options.Create(
        new GithubDataSourceSettings
        {
            RepositoryName = "test-repo",
            RepositoryOwner = "test-owner"
        });

    private GithubFileLoader _loader = null!;
    private IRepositoryContentsClient _repositoryContentsClient = null!;

    [SetUp]
    public void Setup()
    {
        var githubClient = Substitute.For<IGitHubClient>();
        var repositoriesClient = Substitute.For<IRepositoriesClient>();
        _repositoryContentsClient = Substitute.For<IRepositoryContentsClient>();

        githubClient.Repository.Returns(repositoriesClient);
        repositoriesClient.Content.Returns(_repositoryContentsClient);

        _loader = new GithubFileLoader(githubClient, _options);
    }

    [Test]
    public async Task DownloadAsync_ShouldCallRepositoryContentsClientWithValuesFromOptionsAndFilePath()
    {
        const string expectedPath = "path/to/file";

        _ = await _loader.DownloadAsync(expectedPath, CancellationToken.None);

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

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(expectedBytes));
            });
    }
}
