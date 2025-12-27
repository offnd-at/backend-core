namespace OffndAt.Services.Api.Endpoints.V1.Themes;

using Application.Themes.Queries.GetSupportedThemes;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using OffndAt.Contracts.Themes;

/// <summary>
///     Exposes an API endpoint for retrieving available themes.
/// </summary>
internal sealed class Get : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                ApiRoutes.Themes.Get,
                async (ISender sender, CancellationToken cancellationToken) =>
                    await Maybe<GetSupportedThemesQuery>
                        .From(new GetSupportedThemesQuery())
                        .BindAsync(query => sender.Send(query, cancellationToken))
                        .MatchAsync(Results.Ok, () => CustomResults.NotFound(DomainErrors.Theme.NoneAvailable)))
            .WithTags(nameof(ApiRoutes.Themes))
            .WithSummary("Get supported themes")
            .WithDescription("Retrieves a list of all supported themes.")
            .Produces<GetSupportedThemesResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
