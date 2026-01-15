using MassTransit;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.IntegrationEvents.LinkCreated;

namespace OffndAt.Services.EventsWorker.Links.IntegrationEvents.LinkCreated;

/// <summary>
///     Represents the <see cref="LinkCreatedIntegrationEvent" /> handler.
/// </summary>
/// <param name="logger">The logger.</param>
public sealed class LinkCreatedIntegrationEventHandler(ILogger<LinkCreatedIntegrationEventHandler> logger)
    : IIntegrationEventConsumer<LinkCreatedIntegrationEvent>
{
    /// <inheritdoc />
    public Task Consume(ConsumeContext<LinkCreatedIntegrationEvent> context)
    {
        logger.LogInformation("Consumed integration event := {@Event}", context.Message);

        return Task.CompletedTask;
    }
}
