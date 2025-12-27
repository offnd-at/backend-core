using MediatR;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Behaviours;

/// <summary>
///     Represents the transaction behavior middleware for the MediatR pipeline.
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
            return await next();
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

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
