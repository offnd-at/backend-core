namespace OffndAt.Services.Api.Controllers.V1;

using Application.Languages.Queries.GetSupportedLanguages;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Contracts.Languages;

public sealed class LanguagesController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet(ApiRoutes.Languages.GetSupported)]
    [ProducesResponseType(typeof(GetSupportedLanguagesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupported() =>
        await Maybe<GetSupportedLanguagesQuery>
            .From(new GetSupportedLanguagesQuery())
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(Ok, () => NotFound(DomainErrors.Language.NoneAvailable));
}
