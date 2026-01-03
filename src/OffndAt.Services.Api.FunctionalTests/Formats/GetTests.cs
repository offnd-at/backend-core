using OffndAt.Services.Api.FunctionalTests.Abstractions;

namespace OffndAt.Services.Api.FunctionalTests.Formats;

internal sealed class GetTests : BaseFunctionalTest
{
    [Test]
    public async Task Get_ShouldReturnOk_WithAllSupportedFormats()
    {
        var response = await HttpClient.GetAsync("v1/formats");

        await Verify(response);
    }
}
