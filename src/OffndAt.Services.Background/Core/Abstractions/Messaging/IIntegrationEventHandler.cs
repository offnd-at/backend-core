namespace OffndAt.Services.Background.Core.Abstractions.Messaging;

using Application.Core.Abstractions.Messaging;
using MediatR;

/// <summary>
///     Represents the integration event handler interface.
/// </summary>
/// <typeparam name="TIntegrationEvent">The integration event type.</typeparam>
public interface IIntegrationEventHandler<in TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent;
