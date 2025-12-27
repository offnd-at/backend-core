using System.Net.Http.Json;
using Abstractions;
using OffndAt.Contracts.Links;


namespace OffndAt.Services.Api.FunctionalTests.Links;internal sealed class GetByPhraseTests : BaseFunctionalTest
{
    [Test]
    public async Task GetByPhrase_ShouldReturnNotFound_WhenLinkWithSpecifiedPhraseDoesNotExist()
    {
        var response = await HttpClient.GetAsync("v1/links/non-existent-phrase");

        _ = await Verify(response)
            .ScrubMembers("traceId", "activityId", "requestId");
    }

    [Test]
    public async Task GetByPhrase_ShouldReturnOk_WhenLinkWithSpecifiedPhraseExists()
    {
        var linkHttpResponse = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                FormatId = 1,
                LanguageId = 0,
                TargetUrl = "http://localhost",
                ThemeId = 0
            });

        var linkResponse = await linkHttpResponse.Content.ReadFromJsonAsync<GenerateLinkResponse>();
        var phrase = linkResponse?.Url.Split('/').Last();

        var response = await HttpClient.GetAsync($"v1/links/{phrase}");

        _ = await Verify(response);
    }
}
