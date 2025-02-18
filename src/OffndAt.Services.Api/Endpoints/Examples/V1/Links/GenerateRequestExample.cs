namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;

using Bogus;
using Domain.Enumerations;
using Extensions;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Links;

/// <summary>
///     Represents the example request for the generate link endpoint.
/// </summary>
internal sealed class GenerateRequestExample : IOpenApiExample<GenerateLinkRequest>
{
    /// <inheritdoc />
    public OpenApiObject GetExample() =>
        new Faker<GenerateLinkRequest>()
            .RuleFor(r => r.TargetUrl, f => f.Internet.Url())
            .RuleFor(r => r.LanguageId, f => f.PickRandom(Language.List.Select(elem => elem.Value)))
            .RuleFor(r => r.FormatId, f => f.PickRandom(Format.List.Select(elem => elem.Value)))
            .RuleFor(r => r.ThemeId, f => f.PickRandom(Theme.List.Select(elem => elem.Value)))
            .Generate()
            .ToOpenApiObject();
}
