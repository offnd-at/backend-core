using System.Text.Json.Nodes;

namespace OffndAt.Services.Api.Endpoints.Examples;

/// <summary>
///     Represents the OpenAPI request or response example.
/// </summary>
internal interface IOpenApiExample
{
    /// <summary>
    ///     Converts the message to an OpenAPI object.
    /// </summary>
    /// <returns>The message, represented as an OpenAPI object.</returns>
    JsonNode GetExample();
}
