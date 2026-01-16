using MassTransit;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Links.IntegrationEvents.LinkVisited;

/// <summary>
///     Handles the <see cref="LinkVisitedIntegrationEvent" /> by recording visit metrics for observability.
/// </summary>
/// <param name="linkMetrics">The collection of link-related metrics.</param>
public sealed class RecordLinkVisitMetricsHandler(ILinkMetrics linkMetrics) : IIntegrationEventConsumer<LinkVisitedIntegrationEvent>
{
    /// <inheritdoc />
    public Task Consume(ConsumeContext<LinkVisitedIntegrationEvent> context)
    {
        var language = Language.FromValueOrFail(context.Message.LanguageId);
        var theme = Theme.FromValueOrFail(context.Message.ThemeId);

        linkMetrics.RecordLinkVisit(language, theme);

        return Task.CompletedTask;
    }
}
