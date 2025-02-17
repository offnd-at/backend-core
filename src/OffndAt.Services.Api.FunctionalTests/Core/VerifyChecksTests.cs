namespace OffndAt.Services.Api.FunctionalTests.Core;

[TestFixture]
internal sealed class VerifyChecksTests
{
    [Test]
    public Task Run() => VerifyChecks.Run();
}
