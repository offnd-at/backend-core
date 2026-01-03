using System.Text.Json.Nodes;
using Bogus;
using OffndAt.Contracts.Links.Responses;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;

/// <summary>
///     Represents the example response for the generate link endpoint.
/// </summary>
internal sealed class GenerateResponseExample : IOpenApiExample<GenerateLinkResponse>
{
    /// <inheritdoc />
    public JsonNode GetExample() =>
        new Faker<GenerateLinkResponse>()
            .RuleFor(r => r.Url, f => f.Internet.Url())
            .RuleFor(r => r.StatsUrl, f => f.Internet.Url())
            .Generate()
            .ToJsonNode();
}
