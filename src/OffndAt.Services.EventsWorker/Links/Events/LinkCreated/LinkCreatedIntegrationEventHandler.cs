namespace OffndAt.Services.EventsWorker.Links.Events.LinkCreated;

using Abstractions.Messaging;
using Application.Links.Events.LinkCreated;
using MassTransit;
using Microsoft.Extensions.Logging;

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
