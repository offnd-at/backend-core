namespace OffndAt.Services.Api.Controllers;

using Contracts;
using Domain.Core.Primitives;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Represents a base API controller.
/// </summary>
/// <param name="mediator">The mediator instance.</param>
[Route("api")]
public abstract class ApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator;

    /// <summary>
    ///     Creates an <see cref="BadRequestObjectResult" /> that produces a <see cref="StatusCodes.Status400BadRequest" />.
    ///     response based on the specified <see cref="Error" />.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The created <see cref="BadRequestObjectResult" /> for the response.</returns>
    protected IActionResult BadRequest(Error error) => BadRequest(new ApiErrorResponse([error]));

    /// <summary>
    ///     Creates an <see cref="OkObjectResult" /> that produces a <see cref="StatusCodes.Status200OK" />.
    /// </summary>
    /// <param name="value">The response value.</param>
    /// <returns>The created <see cref="OkObjectResult" /> for the response.</returns>
    protected new IActionResult Ok(object value) => base.Ok(value);

    /// <summary>
    ///     Creates an <see cref="NotFoundResult" /> that produces a <see cref="StatusCodes.Status404NotFound" />.
    /// </summary>
    /// <returns>The created <see cref="NotFoundResult" /> for the response.</returns>
    protected new IActionResult NotFound() => base.NotFound();

    /// <summary>
    ///     Creates a <see cref="RedirectResult" /> object with <see cref="RedirectResult.Permanent" /> set to true
    ///     (<see cref="StatusCodes.Status301MovedPermanently" />) using the specified <paramref name="url" />.
    /// </summary>
    /// <param name="url">The URL to redirect to.</param>
    /// <returns>The created <see cref="RedirectResult" /> for the response.</returns>
    protected new IActionResult RedirectPermanent(string url) => base.RedirectPermanent(url);
}
