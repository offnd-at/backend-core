using MassTransit;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.IntegrationEvents.LinkVisited;

namespace OffndAt.Services.EventsWorker.Links.IntegrationEvents.LinkVisited;

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
