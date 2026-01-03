using System.Text.Json.Nodes;
using Bogus;
using OffndAt.Contracts.Formats.Responses;
using OffndAt.Services.Api.Endpoints.Examples.Fakers;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api.Endpoints.Examples.V1.Formats;

/// <summary>
///     Represents the example response for the get supported formats endpoint.
/// </summary>
internal sealed class GetResponseExample : IOpenApiExample<GetSupportedFormatsResponse>
{
    /// <inheritdoc />
    public JsonNode GetExample() =>
        new Faker<GetSupportedFormatsResponse>()
            .RuleFor(r => r.Formats, _ => FormatFakers.Format.Generate(2))
            .Generate()
            .ToJsonNode();
}
