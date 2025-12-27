namespace OffndAt.Application.Core.Abstractions.Messaging;

using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Defines the contract for handling CQRS command operations.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The command response type.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
