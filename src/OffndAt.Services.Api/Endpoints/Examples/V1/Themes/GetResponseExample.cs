using System.Text.Json.Nodes;
using Bogus;
using OffndAt.Contracts.Themes.Responses;
using OffndAt.Services.Api.Endpoints.Examples.Fakers;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api.Endpoints.Examples.V1.Themes;

/// <summary>
///     Represents the example response for the get supported themes endpoint.
/// </summary>
internal sealed class GetResponseExample : IOpenApiExample<GetSupportedThemesResponse>
{
    /// <inheritdoc />
    public JsonNode GetExample() =>
        new Faker<GetSupportedThemesResponse>()
            .RuleFor(r => r.Themes, _ => ThemeFakers.Theme.Generate(2))
            .Generate()
            .ToJsonNode();
}
