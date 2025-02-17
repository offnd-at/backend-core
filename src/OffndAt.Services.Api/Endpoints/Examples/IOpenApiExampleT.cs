namespace OffndAt.Services.Api.Endpoints.Examples;

/// <summary>
///     Represents the OpenAPI request or response example.
/// </summary>
/// <typeparam name="TMessage">The request or response type.</typeparam>
internal interface IOpenApiExample<in TMessage> : IOpenApiExample;
