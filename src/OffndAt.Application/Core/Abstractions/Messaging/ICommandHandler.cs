namespace OffndAt.Application.Core.Abstractions.Messaging;

using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Defines the contract for handling CQRS command operations.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand;
