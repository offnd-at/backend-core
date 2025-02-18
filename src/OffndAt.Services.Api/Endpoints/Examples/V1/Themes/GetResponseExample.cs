namespace OffndAt.Services.Api.Endpoints.Examples.V1.Themes;

using Bogus;
using Extensions;
using Fakers;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Themes;

/// <summary>
///     Represents the example response for the get supported themes endpoint.
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
