namespace OffndAt.Services.Api.Endpoints.V1.Languages;

using Application.Languages.Queries.GetSupportedLanguages;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using OffndAt.Contracts.Languages;

/// <summary>
///     Represents the get supported languages endpoint.
/// </summary>
internal sealed class Get : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                ApiRoutes.Languages.Get,
                async (ISender sender, CancellationToken cancellationToken) =>
                    await Maybe<GetSupportedLanguagesQuery>
                        .From(new GetSupportedLanguagesQuery())
                        .BindAsync(query => sender.Send(query, cancellationToken))
                        .MatchAsync(Results.Ok, () => CustomResults.NotFound(DomainErrors.Language.NoneAvailable)))
            .WithTags(nameof(ApiRoutes.Languages))
            .WithSummary("Get supported languages")
            .WithDescription("Retrieves a list of all supported languages.")
            .Produces<GetSupportedLanguagesResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
