using Application.Formats.Queries.GetSupportedFormats;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using OffndAt.Contracts.Formats;


namespace OffndAt.Services.Api.Endpoints.V1.Formats;/// <summary>
///     Exposes an API endpoint for retrieving available phrase formats.
/// </summary>
internal sealed class Get : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(
                ApiRoutes.Formats.Get,
                async (ISender sender, CancellationToken cancellationToken) =>
                    await Maybe<GetSupportedFormatsQuery>
                        .From(new GetSupportedFormatsQuery())
                        .BindAsync(query => sender.Send(query, cancellationToken))
                        .MatchAsync(Results.Ok, () => CustomResults.NotFound(DomainErrors.Format.NoneAvailable)))
            .WithTags(nameof(ApiRoutes.Formats))
            .WithSummary("Get supported formats")
            .WithDescription("Retrieves a list of all supported formats.")
            .Produces<GetSupportedFormatsResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
}
