using MediatR;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Abstractions.Messaging;

/// <summary>
///     Represents the command handler interface.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The command response type.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;
