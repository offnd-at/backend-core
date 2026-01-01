using MassTransit;
using Microsoft.Extensions.Logging;
using OffndAt.Application.Links.Events.LinkVisited;
using OffndAt.Services.EventsWorker.Abstractions.Messaging;

namespace OffndAt.Services.EventsWorker.Links.Events.LinkVisited;

/// <summary>
///     Represents the <see cref="LinkVisitedIntegrationEvent" /> handler.
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
