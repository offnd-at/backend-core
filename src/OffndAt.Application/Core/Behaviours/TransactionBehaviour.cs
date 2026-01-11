using MediatR;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Behaviours;

/// <summary>
///     Represents the transaction behaviour middleware.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class TransactionBehaviour<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is IQuery<TResponse>)
        {
            return await next(cancellationToken);
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);

            if (response is Result { IsFailure: true })
            {
                await transaction.RollbackAsync(cancellationToken);

                return response;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return response;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}
