using MediatR;


namespace OffndAt.Application.Core.Abstractions.Messaging;/// <summary>
///     Marks classes as integration events for cross-service communication.
/// </summary>
public interface IIntegrationEvent : INotification;
