using MassTransit;
using Microsoft.Extensions.Logging;
using OffndAt.Application.Links.Events.LinkCreated;
using OffndAt.Services.EventsWorker.Abstractions.Messaging;

namespace OffndAt.Services.EventsWorker.Links.Events.LinkCreated;

/// <summary>
///     Handles the LinkCreatedIntegrationEvent for processing new link notifications.
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
