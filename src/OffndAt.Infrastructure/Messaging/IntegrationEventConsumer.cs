namespace OffndAt.Infrastructure.Messaging;

using Application.Core.Abstractions.Messaging;
using MediatR;

/// <summary>
///     Represents the integration event consumer.
/// </summary>
internal sealed class IntegrationEventConsumer(IPublisher mediator) : IIntegrationEventConsumer
{
    /// <inheritdoc />
    public async Task ConsumeAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) =>
        await mediator.Publish(integrationEvent, cancellationToken);
}
