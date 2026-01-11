using MediatR;

namespace OffndAt.Application.Abstractions.Messaging;

/// <summary>
///     Represents the marker interface for an integration event.
/// </summary>
public interface IIntegrationEvent : INotification;
