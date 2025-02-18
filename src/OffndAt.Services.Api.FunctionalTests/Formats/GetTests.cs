namespace OffndAt.Services.Api.FunctionalTests.Formats;

using Abstractions;

internal sealed class GetTests : BaseFunctionalTest
{
    [Test]
    public async Task Get_ShouldReturnOk_WithAllSupportedFormats()
    {
        var response = await HttpClient.GetAsync("v1/formats");

        _ = await Verify(response);
    }
}
