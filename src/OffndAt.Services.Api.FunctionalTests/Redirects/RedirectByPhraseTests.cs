namespace OffndAt.Services.Api.FunctionalTests.Redirects;

using System.Net.Http.Json;
using Abstractions;
using OffndAt.Contracts.Links;

internal sealed class RedirectByPhraseTests : BaseFunctionalTest
{
    [Test]
    public async Task RedirectByPhrase_ShouldReturnNotFound_WhenLinkWithSpecifiedPhraseDoesNotExist()
    {
        var response = await HttpClient.GetAsync("v1/redirects/non-existent-phrase");

        _ = await Verify(response)
            .ScrubMembers("traceId", "activityId", "requestId");
    }

    [Test]
    public async Task RedirectByPhrase_ShouldReturnMovedPermanently_WhenLinkWithSpecifiedPhraseExists()
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

        var response = await HttpClientWithoutRedirects.GetAsync($"v1/redirects/{phrase}");

        _ = await Verify(response)
            .ScrubMember("RequestMessage");
    }
}
