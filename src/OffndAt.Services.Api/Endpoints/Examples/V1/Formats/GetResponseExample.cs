namespace OffndAt.Services.Api.Endpoints.Examples.V1.Formats;

using Bogus;
using Extensions;
using Fakers;
using Microsoft.OpenApi.Any;
using OffndAt.Contracts.Formats;

/// <summary>
///     Provides sample response data for the get supported formats endpoint documentation.
/// </summary>
internal sealed class GetResponseExample : IOpenApiExample<GetSupportedFormatsResponse>
{
    /// <inheritdoc />
    public OpenApiObject GetExample() =>
        new Faker<GetSupportedFormatsResponse>()
            .RuleFor(r => r.Formats, _ => FormatFakers.Format.Generate(2))
            .Generate()
            .ToOpenApiObject();
}
