using MediatR;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Abstractions.Messaging;

/// <summary>
///     Defines the contract for CQRS query operations.
/// </summary>
/// <typeparam name="TResponse">The query response type.</typeparam>
public interface IQuery<TResponse> : IRequest<Maybe<TResponse>>;
