using MassTransit;
using OffndAt.Application.Core.Abstractions.Messaging;

namespace OffndAt.Services.EventsWorker.Abstractions.Messaging;

/// <summary>
///     Defines the contract for consuming integration events from the message broker.
/// </summary>
/// <typeparam name="TIntegrationEvent">The integration event type.</typeparam>
public interface IIntegrationEventConsumer<in TIntegrationEvent> : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent;
