using Application.Core.Abstractions.Messaging;
using MassTransit;


namespace OffndAt.Infrastructure.Core.Messaging;/// <summary>
///     Provides functionality for publishing integration events to RabbitMQ.
/// </summary>
/// <param name="publishEndpoint">The integration event publish endpoint.</param>
internal sealed class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class =>
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
}
