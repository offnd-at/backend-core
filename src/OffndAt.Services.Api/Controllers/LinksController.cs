namespace OffndAt.Services.Api.Controllers;

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPhrase(string phrase) =>
        await Maybe<GetLinkByPhraseQuery>
            .From(new GetLinkByPhraseQuery(phrase))
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(Ok, NotFound);

    [HttpGet(ApiRoutes.Links.RedirectByPhrase)]
    [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectByPhrase(string phrase) =>
        await Maybe<GetLinkByPhraseQuery>
            .From(new GetLinkByPhraseQuery(phrase))
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(response => RedirectPermanent(response.Link.TargetUrl), NotFound);
}
