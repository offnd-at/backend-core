namespace OffndAt.Application.Core.Behaviours;

using Domain.Core.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
///     Represents the logging behaviour middleware.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (response is Result { IsFailure: true } result)
        {
            logger.LogWarning(
                "Error encountered in the result of request := {RequestName}, error := {@Error}",
                typeof(TRequest).Name,
                result.Error);
        }

        return response;
    }
}
