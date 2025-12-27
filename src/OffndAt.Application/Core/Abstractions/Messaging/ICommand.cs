using MediatR;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Core.Abstractions.Messaging;

/// <summary>
///     Defines the contract for CQRS command operations without return values.
/// </summary>
public interface ICommand : IRequest<Result>;
