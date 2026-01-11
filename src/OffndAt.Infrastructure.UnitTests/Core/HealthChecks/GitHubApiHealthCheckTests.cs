using System.Net;
using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OffndAt.Infrastructure.Core.HealthChecks;

namespace OffndAt.Infrastructure.UnitTests.Core.HealthChecks;

internal sealed class GitHubApiHealthCheckTests
{
    private static GitHubApiHealthCheck CreateSut(string responseContent)
    {
        var handler = new FakeHttpMessageHandler(responseContent);
        var httpClient = new HttpClient(handler);

        return new GitHubApiHealthCheck(httpClient);
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnHealthy_WhenStatusIsNone()
    {
        var sut = CreateSut(
            """
            {
              "status": { "indicator": "none" }
            }
            """);

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Healthy));
            Assert.That(actual.Description, Is.Null);
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnDegraded_WhenStatusIsMinor()
    {
        var sut = CreateSut(
            """
            {
              "status": { "indicator": "minor" }
            }
            """);

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Degraded));
            Assert.That(actual.Description, Is.EqualTo("GitHub API is experiencing minor outage."));
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnDegraded_WhenStatusIsMajor()
    {
        var sut = CreateSut(
            """
            {
              "status": { "indicator": "major" }
            }
            """);

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Degraded));
            Assert.That(actual.Description, Is.EqualTo("GitHub API is experiencing major outage."));
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnUnhealthy_WhenStatusIsCritical()
    {
        var sut = CreateSut(
            """
            {
              "status": { "indicator": "critical" }
            }
            """);

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(actual.Description, Is.EqualTo("GitHub API is experiencing critical outage."));
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnUnhealthy_WhenStatusIsUnknown()
    {
        var sut = CreateSut(
            """
            {
              "status": { "indicator": "unknown" }
            }
            """);

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(actual.Description, Is.EqualTo("GitHub API is unavailable."));
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnUnhealthy_WhenStatusIsNotPresent()
    {
        var sut = CreateSut(
            """
            {
            }
            """);

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(actual.Description, Is.EqualTo("GitHub API is unavailable."));
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnUnhealthy_WhenRequestTimesOut()
    {
        var handler = new ThrowingHttpMessageHandler(new TaskCanceledException("timeout"));

        var sut = new GitHubApiHealthCheck(new HttpClient(handler));

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(actual.Description, Is.EqualTo("GitHub API health check timed out."));
            Assert.That(actual.Exception, Is.TypeOf<TaskCanceledException>());
        });
    }

    [Test]
    public async Task CheckHealthAsync_ShouldReturnUnhealthy_WhenExceptionOccurs()
    {
        var handler = new ThrowingHttpMessageHandler(new InvalidOperationException("boom"));

        var sut = new GitHubApiHealthCheck(new HttpClient(handler));

        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(actual.Status, Is.EqualTo(HealthStatus.Unhealthy));
            Assert.That(actual.Description, Is.EqualTo("GitHub API health check failed."));
            Assert.That(actual.Exception, Is.TypeOf<InvalidOperationException>());
        });
    }

    private sealed class FakeHttpMessageHandler(string response)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(response, Encoding.UTF8, "application/json")
            };

            return Task.FromResult(message);
        }
    }

    private sealed class ThrowingHttpMessageHandler(Exception exception)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw exception;
    }
}
