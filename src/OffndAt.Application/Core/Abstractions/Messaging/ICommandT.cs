namespace OffndAt.Application.Core.Abstractions.Messaging;

using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Defines the contract for CQRS command operations without return values.
/// </summary>
/// <typeparam name="TResponse">The command response type.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
