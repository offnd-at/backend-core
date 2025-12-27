using Bogus;
using Extensions;
using Fakers;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Themes;


namespace OffndAt.Services.Api.Endpoints.Examples.V1.Themes;/// <summary>
///     Provides sample response data for the get supported themes endpoint documentation.
/// </summary>
internal sealed class GetResponseExample : IOpenApiExample<GetSupportedThemesResponse>
{
    /// <inheritdoc />
    public OpenApiObject GetExample() =>
        new Faker<GetSupportedThemesResponse>()
            .RuleFor(r => r.Themes, _ => ThemeFakers.Theme.Generate(2))
            .Generate()
            .ToOpenApiObject();
}
