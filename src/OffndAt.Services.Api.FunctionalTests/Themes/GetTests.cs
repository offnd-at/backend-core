using OffndAt.Services.Api.FunctionalTests.Abstractions;

namespace OffndAt.Services.Api.FunctionalTests.Themes;

internal sealed class GetTests : BaseFunctionalTest
{
    [Test]
    public async Task Get_ShouldReturnOk_WithAllSupportedThemes()
    {
        var response = await HttpClient.GetAsync("v1/themes");

        _ = await Verify(response);
    }
}
