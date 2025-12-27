using Domain.Core.Primitives;
using MediatR;


namespace OffndAt.Application.Core.Abstractions.Messaging;/// <summary>
///     Defines the contract for CQRS query operations.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResponse">The query response type.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Maybe<TResponse>>
    where TQuery : IQuery<TResponse>;
