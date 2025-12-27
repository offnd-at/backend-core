namespace OffndAt.Services.Api.Endpoints;

/// <summary>
///     Defines the contract for API endpoint registration and configuration.
/// </summary>
internal interface IEndpoint
{
    /// <summary>
    ///     Maps the endpoint with the endpoint route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
