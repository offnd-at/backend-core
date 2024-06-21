namespace OffndAt.Services.Background.Links.Events;

using Application.Links.Events.LinkVisited;
using Core.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

/// <summary>
///     Represents the <see cref="LinkVisitedIntegrationEvent" /> handler.
/// </summary>
/// <param name="logger">The logger.</param>
internal sealed class LinkVisitedIntegrationEventHandler(ILogger<LinkVisitedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<LinkVisitedIntegrationEvent>
{
    /// <inheritdoc />
    public Task Handle(LinkVisitedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consumed integration event := {@Event}", notification);

        return Task.CompletedTask;
    }
}
