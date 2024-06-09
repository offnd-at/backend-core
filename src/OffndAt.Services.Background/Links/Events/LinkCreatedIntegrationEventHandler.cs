namespace OffndAt.Services.Background.Links.Events;

using Application.Links.Events.LinkCreated;
using Core.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

/// <summary>
///     Represents the <see cref="LinkCreatedIntegrationEvent" /> handler.
/// </summary>
/// <param name="logger">The logger.</param>
internal sealed class LinkCreatedIntegrationEventHandler(ILogger<LinkCreatedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<LinkCreatedIntegrationEvent>
{
    /// <inheritdoc />
    public Task Handle(LinkCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consumed integration event := {@Event}", notification);

        return Task.CompletedTask;
    }
}
