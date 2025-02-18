namespace OffndAt.Infrastructure.Core.Messaging;

using Application.Core.Abstractions.Messaging;
using MassTransit;

/// <summary>
///     Represents the integration event publisher.
/// </summary>
/// <param name="publishEndpoint">The integration event publish endpoint.</param>
internal sealed class IntegrationEventPublisher(IPublishEndpoint publishEndpoint) : IIntegrationEventPublisher
{
    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default) where TEvent : class =>
        await publishEndpoint.Publish(integrationEvent, cancellationToken);
}
