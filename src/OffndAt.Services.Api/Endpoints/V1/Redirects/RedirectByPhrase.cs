namespace OffndAt.Services.Api.Endpoints.V1.Redirects;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Application.Links.Queries.GetLinkByPhrase;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Exposes an API endpoint for redirecting users via shortened link phrases.
/// </summary>
internal sealed class RedirectByPhrase : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                ApiRoutes.Redirects.RedirectByPhrase,
                async (
                        [Description("The phrase of the shortened URL to search for.")][Required] string phrase,
                        ISender sender,
                        HttpContext httpContext,
                        CancellationToken cancellationToken) =>
                    await Maybe<GetLinkByPhraseQuery>
                        .From(new GetLinkByPhraseQuery(HttpUtility.UrlDecode(phrase), true))
                        .BindAsync(query => sender.Send(query, cancellationToken))
                        .MatchAsync(
                            response =>
                            {
                                httpContext.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
                                httpContext.Response.Headers.Pragma = "no-cache";
                                httpContext.Response.Headers.Expires = "0";

                                return Results.Redirect(response.Link.TargetUrl, true);
                            },
                            () => CustomResults.NotFound(DomainErrors.Link.NotFound)))
            .WithTags(nameof(ApiRoutes.Redirects))
            .WithSummary("Redirect by phrase")
            .WithDescription("Retrieves a link using its unique phrase and redirects to its target URL.")
            .Produces(StatusCodes.Status301MovedPermanently)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
