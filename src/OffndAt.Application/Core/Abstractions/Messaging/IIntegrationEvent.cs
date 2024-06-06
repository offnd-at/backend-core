namespace OffndAt.Application.Core.Abstractions.Messaging;

using MediatR;

/// <summary>
///     Represents the marker interface for an integration event.
/// </summary>
public interface IIntegrationEvent : INotification;
