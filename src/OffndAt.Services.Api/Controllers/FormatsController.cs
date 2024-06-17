namespace OffndAt.Services.Api.Controllers;

using Application.Formats.Queries.GetSupportedFormats;
using Contracts;
using Domain.Core.Errors;
using Domain.Core.Extensions;
using Domain.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Contracts.Formats;

public sealed class FormatsController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet(ApiRoutes.Formats.GetSupported)]
    [ProducesResponseType(typeof(GetSupportedFormatsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSupported() =>
        await Maybe<GetSupportedFormatsQuery>
            .From(new GetSupportedFormatsQuery())
            .BindAsync(query => Mediator.Send(query))
            .MatchAsync(Ok, () => NotFound(DomainErrors.Format.NoneAvailable));
}
