﻿namespace OffndAt.Services.Api.FunctionalTests.Links;

using System.Net.Http.Json;
using Abstractions;
using OffndAt.Contracts.Links;

internal sealed class GenerateTests : BaseFunctionalTest
{
    [Test]
    public async Task Generate_ShouldReturnCreated_WhenSuccessfullyCreatesLink()
    {
        var response = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                FormatId = 1,
                LanguageId = 0,
                TargetUrl = "http://localhost",
                ThemeId = 0
            });

        _ = await Verify(response)
            .ScrubMember("url")
            .ScrubMember("statsUrl")
            .ScrubMember("Location");
    }

    [Test]
    public async Task Generate_ShouldReturnBadRequest_WhenTargetUrlWasNotProvided()
    {
        var response = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                FormatId = 1,
                LanguageId = 0,
                ThemeId = 0
            });

        _ = await Verify(response)
            .ScrubMember("message")
            .ScrubMember("traceId")
            .ScrubMember("activityId")
            .ScrubMember("requestId");
    }

    [Test]
    public async Task Generate_ShouldReturnBadRequest_WhenTargetUrlWasNotValidUri()
    {
        var response = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                FormatId = 1,
                LanguageId = 0,
                TargetUrl = "testString",
                ThemeId = 0
            });

        _ = await Verify(response)
            .ScrubMember("message")
            .ScrubMember("traceId")
            .ScrubMember("activityId")
            .ScrubMember("requestId");
    }

    [Test]
    public async Task Generate_ShouldReturnBadRequest_WhenLanguageIdWasNotProvided()
    {
        var response = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                TargetUrl = "http://localhost",
                FormatId = 1,
                ThemeId = 0
            });

        _ = await Verify(response)
            .ScrubMember("message")
            .ScrubMember("traceId")
            .ScrubMember("activityId")
            .ScrubMember("requestId");
    }

    [Test]
    public async Task Generate_ShouldReturnBadRequest_WhenThemeIdWasNotProvided()
    {
        var response = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                TargetUrl = "http://localhost",
                FormatId = 1,
                LanguageId = 0
            });

        _ = await Verify(response)
            .ScrubMember("message")
            .ScrubMember("traceId")
            .ScrubMember("activityId")
            .ScrubMember("requestId");
    }

    [Test]
    public async Task Generate_ShouldReturnBadRequest_WhenFormatIdWasNotProvided()
    {
        var response = await HttpClient.PostAsJsonAsync(
            "v1/links",
            new GenerateLinkRequest
            {
                TargetUrl = "http://localhost",
                LanguageId = 0,
                ThemeId = 0
            });

        _ = await Verify(response)
            .ScrubMember("message")
            .ScrubMember("traceId")
            .ScrubMember("activityId")
            .ScrubMember("requestId");
    }
}
