using MediatR;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Abstractions.Messaging;

/// <summary>
///     Represents the command interface.
/// </summary>
public interface ICommand : IRequest<Result>;
