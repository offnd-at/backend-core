using MediatR;
using OffndAt.Application.Languages.Queries.GetSupportedLanguages;
using OffndAt.Contracts.Languages;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Extensions;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Services.Api.Contracts;

namespace OffndAt.Services.Api.Endpoints.V1.Languages;

/// <summary>
///     Exposes an API endpoint for retrieving available languages.
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
