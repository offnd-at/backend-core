using MassTransit;
using OffndAt.Application.Abstractions.Messaging;

namespace OffndAt.Infrastructure.Core.Messaging;

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
