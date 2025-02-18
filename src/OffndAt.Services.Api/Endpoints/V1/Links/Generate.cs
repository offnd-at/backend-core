namespace OffndAt.Services.Api.Endpoints.V1.Links;

using Application.Links.Commands.GenerateLink;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using OffndAt.Contracts.Links;

/// <summary>
///     Represents the generate link endpoint.
/// </summary>
internal sealed class Generate : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(
                ApiRoutes.Links.Generate,
                async (
                        GenerateLinkRequest generateLinkRequest,
                        ISender sender,
                        CancellationToken cancellationToken) =>
                    await Result.Create(generateLinkRequest, DomainErrors.General.UnprocessableRequest)
                        .Map(
                            request => new GenerateLinkCommand(
                                request.TargetUrl ?? string.Empty,
                                request.LanguageId ?? -1,
                                request.ThemeId ?? -1,
                                request.FormatId ?? -1))
                        .BindAsync(command => sender.Send(command, cancellationToken))
                        .MatchAsync(response => Results.Created(new Uri(response.Url), response), CustomResults.BadRequest))
            .WithTags(nameof(ApiRoutes.Links))
            .WithSummary("Generate a link")
            .WithDescription("Generates a new link using the provided details.")
            .Produces<GenerateLinkResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
