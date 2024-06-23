namespace OffndAt.Services.Api.Controllers.V1;

using System.Web;
using Application.Links.Commands.GenerateLink;
using Application.Links.Queries.GetLinkByPhrase;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Contracts.Links;

public sealed class LinksController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost(ApiRoutes.Links.Generate)]
    [ProducesResponseType(typeof(GenerateLinkResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(GenerateLinkRequest generateLinkRequest) =>
        await Result.Create(generateLinkRequest, DomainErrors.General.UnprocessableRequest)
            .Map(
                request => new GenerateLinkCommand(
                    request.TargetUrl,
                    request.LanguageId,
                    request.ThemeId,
                    request.FormatId))
            .BindAsync(command => Mediator.Send(command))
            .MatchAsync(response => Created(new Uri(response.Url), response), BadRequest);

    [HttpGet(ApiRoutes.Links.GetByPhrase)]
    [ProducesResponseType(typeof(GetLinkByPhraseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPhrase(string phrase) =>
        await Maybe<GetLinkByPhraseQuery>
            .From(new GetLinkByPhraseQuery(phrase, false))
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(Ok, () => NotFound(DomainErrors.Link.NotFound));

    [HttpGet(ApiRoutes.Links.RedirectByPhrase)]
    [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ResponseCache(Location = ResponseCacheLocation.None, Duration = 0)]
    public async Task<IActionResult> RedirectByPhrase(string phrase) =>
        await Maybe<GetLinkByPhraseQuery>
            .From(new GetLinkByPhraseQuery(HttpUtility.UrlDecode(phrase), true))
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(response => RedirectPermanent(response.Link.TargetUrl), () => NotFound(DomainErrors.Link.NotFound));
}
