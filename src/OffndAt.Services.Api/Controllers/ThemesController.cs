namespace OffndAt.Services.Api.Controllers;

using Application.Themes.Queries.GetSupportedThemes;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Contracts.Themes;

public sealed class ThemesController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet(ApiRoutes.Themes.GetSupported)]
    [ProducesResponseType(typeof(GetSupportedThemesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupported() =>
        await Maybe<GetSupportedThemesQuery>
            .From(new GetSupportedThemesQuery())
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(Ok, () => NotFound(DomainErrors.Theme.NoneAvailable));
}
