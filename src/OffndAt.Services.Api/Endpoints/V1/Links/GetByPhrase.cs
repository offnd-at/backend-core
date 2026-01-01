using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MediatR;
using OffndAt.Application.Links.Queries.GetLinkByPhrase;
using OffndAt.Contracts.Links;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Services.Api.Contracts;

namespace OffndAt.Services.Api.Endpoints.V1.Links;

/// <summary>
///     Represents the get link by phrase endpoint.
/// </summary>
internal sealed class GetByPhrase : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                ApiRoutes.Links.GetByPhrase,
                async (
                        [Description("The phrase of the shortened URL to search for.")] [Required] string phrase,
                        ISender sender,
                        CancellationToken cancellationToken) =>
                    await Maybe<GetLinkByPhraseQuery>
                        .From(new GetLinkByPhraseQuery(phrase, false))
                        .BindAsync(query => sender.Send(query, cancellationToken))
                        .MatchAsync(Results.Ok, () => CustomResults.NotFound(DomainErrors.Link.NotFound)))
            .WithTags(nameof(ApiRoutes.Links))
            .WithSummary("Get link by phrase")
            .WithDescription("Retrieves a link using its unique phrase.")
            .Produces<GetLinkByPhraseResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
