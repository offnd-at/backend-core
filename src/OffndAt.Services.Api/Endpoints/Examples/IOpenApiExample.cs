using Microsoft.OpenApi.Any;


namespace OffndAt.Services.Api.Endpoints.Examples;/// <summary>
///     Defines the contract for providing OpenAPI request or response examples.
/// </summary>
internal interface IOpenApiExample
{
    /// <summary>
    ///     Converts the message to an OpenAPI object.
    /// </summary>
    /// <returns>The message, represented as an OpenAPI object.</returns>
    OpenApiObject GetExample();
}
