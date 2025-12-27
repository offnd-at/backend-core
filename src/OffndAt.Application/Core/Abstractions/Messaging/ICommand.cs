namespace OffndAt.Application.Core.Abstractions.Messaging;

using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Defines the contract for CQRS command operations without return values.
/// </summary>
public interface ICommand : IRequest<Result>;
