namespace OffndAt.Services.Api.Endpoints.Examples;

/// <summary>
///     Defines the contract for providing OpenAPI request or response examples.
/// </summary>
/// <typeparam name="TMessage">The request or response type.</typeparam>
internal interface IOpenApiExample<in TMessage> : IOpenApiExample;
