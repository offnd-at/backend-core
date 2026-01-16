using MassTransit;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Domain.Enumerations;

namespace OffndAt.Application.Links.IntegrationEvents.LinkCreated;

/// <summary>
///     Handles the <see cref="LinkCreatedIntegrationEvent" /> by recording creation metrics for observability.
/// </summary>
/// <param name="linkMetrics">The collection of link-related metrics.</param>
public sealed class RecordLinkCreationMetricsHandler(ILinkMetrics linkMetrics) : IIntegrationEventConsumer<LinkCreatedIntegrationEvent>
{
    /// <inheritdoc />
    public Task Consume(ConsumeContext<LinkCreatedIntegrationEvent> context)
    {
        var language = Language.FromValueOrFail(context.Message.LanguageId);
        var theme = Theme.FromValueOrFail(context.Message.ThemeId);

        linkMetrics.RecordLinkCreation(language, theme);
        return Task.CompletedTask;
    }
}
