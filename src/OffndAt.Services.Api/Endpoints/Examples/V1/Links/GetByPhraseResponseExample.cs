namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;

using Bogus;
using Extensions;
using Fakers;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Links;

/// <summary>
///     Provides sample response data for the get link by phrase endpoint documentation.
/// </summary>
internal sealed class GetByPhraseResponseExample : IOpenApiExample<GetLinkByPhraseResponse>
{
    /// <inheritdoc />
    public OpenApiObject GetExample() =>
        new Faker<GetLinkByPhraseResponse>()
            .RuleFor(r => r.Link, _ => LinkFakers.Link.Generate())
            .Generate()
            .ToOpenApiObject();
}
