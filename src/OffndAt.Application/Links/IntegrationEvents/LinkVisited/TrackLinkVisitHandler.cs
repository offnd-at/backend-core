using MassTransit;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Links.IntegrationEvents.LinkVisited;

/// <summary>
///     Handles the <see cref="LinkVisitedIntegrationEvent" /> by caching visit data for aggregation.
/// </summary>
/// <param name="linkVisitTracker">The link visit tracker.</param>
public sealed class TrackLinkVisitHandler(ILinkVisitTracker linkVisitTracker) : IIntegrationEventConsumer<LinkVisitedIntegrationEvent>
{
    /// <inheritdoc />
    public async Task Consume(ConsumeContext<LinkVisitedIntegrationEvent> context) =>
        await linkVisitTracker.RecordAsync(new LinkId(context.Message.LinkId), context.CancellationToken);
}
