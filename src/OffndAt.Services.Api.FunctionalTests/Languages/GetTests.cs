using OffndAt.Services.Api.FunctionalTests.Abstractions;

namespace OffndAt.Services.Api.FunctionalTests.Languages;

internal sealed class GetTests : BaseFunctionalTest
{
    [Test]
    public async Task Get_ShouldReturnOk_WithAllSupportedLanguages()
    {
        var response = await HttpClient.GetAsync("v1/languages");

        _ = await Verify(response);
    }
}
