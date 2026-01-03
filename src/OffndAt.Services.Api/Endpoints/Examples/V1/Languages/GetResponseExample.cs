using System.Text.Json.Nodes;
using Bogus;
using OffndAt.Contracts.Languages.Responses;
using OffndAt.Services.Api.Endpoints.Examples.Fakers;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api.Endpoints.Examples.V1.Languages;

/// <summary>
///     Represents the example response for the get supported languages endpoint.
/// </summary>
internal sealed class GetResponseExample : IOpenApiExample<GetSupportedLanguagesResponse>
{
    /// <inheritdoc />
    public JsonNode GetExample() =>
        new Faker<GetSupportedLanguagesResponse>()
            .RuleFor(r => r.Languages, _ => LanguageFakers.Language.Generate(2))
            .Generate()
            .ToJsonNode();
}
