using System.Text.Json.Nodes;
using Bogus;
using OffndAt.Contracts.Links.Responses;
using OffndAt.Services.Api.Endpoints.Examples.Fakers;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;

/// <summary>
///     Represents the example response for the get link by phrase endpoint.
/// </summary>
internal sealed class GetByPhraseResponseExample : IOpenApiExample<GetLinkByPhraseResponse>
{
    /// <inheritdoc />
    public JsonNode GetExample() =>
        new Faker<GetLinkByPhraseResponse>()
            .RuleFor(r => r.Link, _ => LinkFakers.Link.Generate())
            .Generate()
            .ToJsonNode();
}
