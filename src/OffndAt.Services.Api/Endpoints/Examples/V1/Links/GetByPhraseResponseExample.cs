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
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.Phrase, f => string.Join(string.Empty, f.Lorem.Words()))
            .RuleFor(r => r.TargetUrl, f => f.Internet.Url())
            .RuleFor(r => r.Visits, f => f.Random.Int(100, 2000))
            .RuleFor(r => r.RecentVisits, _ => LinkFakers.LinkVisit.Generate(2))
            .RuleFor(r => r.CreatedAt, f => f.Date.RecentOffset())
            .Generate()
            .ToJsonNode();
}
