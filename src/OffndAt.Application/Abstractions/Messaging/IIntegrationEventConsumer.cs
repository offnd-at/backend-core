using MassTransit;

namespace OffndAt.Application.Abstractions.Messaging;

/// <summary>
///     Represents the integration event consumer interface.
/// </summary>
/// <typeparam name="TIntegrationEvent">The integration event type.</typeparam>
public interface IIntegrationEventConsumer<in TIntegrationEvent> : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent;
