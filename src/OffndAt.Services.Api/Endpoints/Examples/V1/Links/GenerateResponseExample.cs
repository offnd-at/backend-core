namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;

using Bogus;
using Extensions;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Links;

/// <summary>
///     Provides sample response data for the generate link endpoint documentation.
/// </summary>
internal sealed class GenerateResponseExample : IOpenApiExample<GenerateLinkResponse>
{
    /// <inheritdoc />
    public OpenApiObject GetExample() =>
        new Faker<GenerateLinkResponse>()
            .RuleFor(r => r.Url, f => f.Internet.Url())
            .RuleFor(r => r.StatsUrl, f => f.Internet.Url())
            .Generate()
            .ToOpenApiObject();
}
