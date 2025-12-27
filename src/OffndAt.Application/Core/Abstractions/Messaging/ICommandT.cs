using MediatR;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Abstractions.Messaging;

/// <summary>
///     Defines the contract for CQRS command operations without return values.
/// </summary>
/// <typeparam name="TResponse">The command response type.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
