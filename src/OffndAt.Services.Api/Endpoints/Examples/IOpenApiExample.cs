namespace OffndAt.Services.Api.Endpoints.Examples;

using Microsoft.OpenApi.Any;

/// <summary>
///     Represents the OpenAPI request or response example.
/// </summary>
internal interface IOpenApiExample
{
    /// <summary>
    ///     Converts the message to an OpenAPI object.
    /// </summary>
    /// <returns>The message, represented as an OpenAPI object.</returns>
    OpenApiObject GetExample();
}
