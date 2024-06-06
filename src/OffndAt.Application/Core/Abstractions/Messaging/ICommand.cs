namespace OffndAt.Application.Core.Abstractions.Messaging;

using Domain.Core.Primitives;
using MediatR;

/// <summary>
///     Represents the command interface.
/// </summary>
public interface ICommand : IRequest<Result>;
