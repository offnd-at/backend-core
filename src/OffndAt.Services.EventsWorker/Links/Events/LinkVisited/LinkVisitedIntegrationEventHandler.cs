namespace OffndAt.Services.EventsWorker.Links.Events.LinkVisited;

using Abstractions.Messaging;
using Application.Links.Events.LinkVisited;
using MassTransit;
using Microsoft.Extensions.Logging;

/// <summary>
///     Handles the LinkVisitedIntegrationEvent for processing link visit notifications.
/// </summary>
/// <param name="logger">The logger.</param>
public sealed class LinkVisitedIntegrationEventHandler(ILogger<LinkVisitedIntegrationEventHandler> logger)
    : IIntegrationEventConsumer<LinkVisitedIntegrationEvent>
{
    /// <inheritdoc />
    public Task Consume(ConsumeContext<LinkVisitedIntegrationEvent> context)
    {
        logger.LogInformation("Consumed integration event := {@Event}", context.Message);

        return Task.CompletedTask;
    }
}
