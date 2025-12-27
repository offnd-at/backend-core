using Bogus;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Languages;
using OffndAt.Services.Api.Endpoints.Examples.Fakers;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api.Endpoints.Examples.V1.Languages;

/// <summary>
///     Provides sample response data for the get supported languages endpoint documentation.
/// </summary>
internal sealed class GetResponseExample : IOpenApiExample<GetSupportedLanguagesResponse>
{
    /// <inheritdoc />
    public OpenApiObject GetExample() =>
        new Faker<GetSupportedLanguagesResponse>()
            .RuleFor(r => r.Languages, _ => LanguageFakers.Language.Generate(2))
            .Generate()
            .ToOpenApiObject();
}
