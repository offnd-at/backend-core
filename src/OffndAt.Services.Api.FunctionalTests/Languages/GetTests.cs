namespace OffndAt.Services.Api.FunctionalTests.Languages;

using Abstractions;

internal sealed class GetTests : BaseFunctionalTest
{
    [Test]
    public async Task Get_ShouldReturnOk_WithAllSupportedLanguages()
    {
        var response = await HttpClient.GetAsync("v1/languages");

        _ = await Verify(response);
    }
}
