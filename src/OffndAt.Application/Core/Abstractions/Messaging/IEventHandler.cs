namespace OffndAt.Application.Core.Abstractions.Messaging;

using MediatR;

/// <summary>
///     Represents the event handler interface.
/// </summary>
/// <typeparam name="TEvent">The event type.</typeparam>
public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification;
