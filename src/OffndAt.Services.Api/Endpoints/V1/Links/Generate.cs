using MediatR;
using OffndAt.Application.Links.Commands.GenerateLink;
using OffndAt.Contracts.Links;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Services.Api.Contracts;

namespace OffndAt.Services.Api.Endpoints.V1.Links;

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
                        .Map(request => new GenerateLinkCommand(
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
