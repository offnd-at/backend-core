namespace OffndAt.Services.Api.Endpoints;

/// <summary>
///     Represents the endpoint interface.
/// </summary>
internal interface IEndpoint
{
    /// <summary>
    ///     Maps the endpoint with the endpoint route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
