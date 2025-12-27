namespace OffndAt.Services.EventsWorker.Abstractions.Messaging;

using Application.Core.Abstractions.Messaging;
using MassTransit;

/// <summary>
///     Defines the contract for consuming integration events from the message broker.
/// </summary>
/// <typeparam name="TIntegrationEvent">The integration event type.</typeparam>
public interface IIntegrationEventConsumer<in TIntegrationEvent> : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent;
