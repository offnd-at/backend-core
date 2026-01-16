using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MediatR;
using OffndAt.Application.Links.Commands.VisitLink;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Infrastructure.Core.Constants;
using OffndAt.Services.Api.Contracts;

namespace OffndAt.Services.Api.Endpoints.V1.Redirects;

/// <summary>
///     Represents the redirect by phrase endpoint.
/// </summary>
internal sealed class RedirectByPhrase : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                ApiRoutes.Redirects.RedirectByPhrase,
                async (
                        [Description("The phrase of the shortened URL to search for.")] [Required] string phrase,
                        ISender sender,
                        HttpContext httpContext,
                        CancellationToken cancellationToken) =>
                    await Result.Create(HttpUtility.UrlDecode(phrase), DomainErrors.General.UnprocessableRequest)
                        .Map(decodedPhrase => new VisitLinkCommand(decodedPhrase))
                        .BindAsync(command => sender.Send(command, cancellationToken))
                        .MatchAsync(
                            url =>
                            {
                                httpContext.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
                                httpContext.Response.Headers.Pragma = "no-cache";
                                httpContext.Response.Headers.Expires = "0";

                                return Results.Redirect(url, true);
                            },
                            CustomResults.BadRequest))
            .RequireRateLimiting(RateLimitingPolicyNames.RedirectByPhrase)
            .WithTags(nameof(ApiRoutes.Redirects))
            .WithSummary("Redirect by phrase")
            .WithDescription("Retrieves a link using its unique phrase and redirects to its target URL.")
            .Produces(StatusCodes.Status301MovedPermanently)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
