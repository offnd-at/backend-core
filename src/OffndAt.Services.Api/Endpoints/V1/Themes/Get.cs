using MediatR;
using OffndAt.Application.Themes.Queries.GetSupportedThemes;
using OffndAt.Contracts.Themes.Responses;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Services.Api.Contracts;

namespace OffndAt.Services.Api.Endpoints.V1.Themes;

/// <summary>
///     Represents the get supported themes endpoint.
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
            .RequireAuthorization()
            .WithTags(nameof(ApiRoutes.Themes))
            .WithSummary("Get supported themes")
            .WithDescription("Retrieves a list of all supported themes.")
            .Produces<GetSupportedThemesResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
