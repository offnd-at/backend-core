namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;

using Bogus;
using Extensions;
using Fakers;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Links;

/// <summary>
///     Represents the example response for the get link by phrase endpoint.
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
