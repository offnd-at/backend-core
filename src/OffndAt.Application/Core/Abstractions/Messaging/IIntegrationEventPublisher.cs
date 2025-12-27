namespace OffndAt.Application.Core.Abstractions.Messaging;

/// <summary>
///     Defines the contract for publishing integration events to the message broker.
/// </summary>
public interface IIntegrationEventPublisher
{
    /// <summary>
    ///     Publishes the specified integration event to the message queue.
    /// </summary>
    /// <param name="integrationEvent">The integration event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TEvent">The integration event type.</typeparam>
    /// <returns>The completed task.</returns>
    Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class;
}
