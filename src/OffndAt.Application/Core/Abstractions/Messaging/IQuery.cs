namespace OffndAt.Application.Core.Abstractions.Messaging;

using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Defines the contract for CQRS query operations.
/// </summary>
/// <typeparam name="TResponse">The query response type.</typeparam>
public interface IQuery<TResponse> : IRequest<Maybe<TResponse>>;
