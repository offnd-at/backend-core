using Bogus;
using Domain.Enumerations;
using Extensions;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Links;


namespace OffndAt.Services.Api.Endpoints.Examples.V1.Links;/// <summary>
///     Provides sample request data for the generate link endpoint documentation.
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
