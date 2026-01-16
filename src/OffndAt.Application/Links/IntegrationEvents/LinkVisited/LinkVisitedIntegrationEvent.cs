using System.Text.Json.Serialization;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.IntegrationEvents.LinkVisited;

/// <summary>
///     Represents an integration event raised when a link is visited.
/// </summary>
public sealed class LinkVisitedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkVisitedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="domainEvent">The link visited domain event.</param>
    internal LinkVisitedIntegrationEvent(LinkVisitedDomainEvent domainEvent)
    {
        LinkId = domainEvent.LinkId;
        LanguageId = domainEvent.Context.Language.Value;
        ThemeId = domainEvent.Context.Theme.Value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkVisitedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <param name="languageId">The language identifier.</param>
    /// <param name="themeId">The theme identifier.</param>
    /// <remarks>
    ///     Required by JSON deserializer.
    /// </remarks>
    [JsonConstructor]
    private LinkVisitedIntegrationEvent(Guid linkId, int languageId, int themeId)
    {
        LinkId = linkId;
        LanguageId = languageId;
        ThemeId = themeId;
    }

    /// <summary>
    ///     Gets the link identifier.
    /// </summary>
    public Guid LinkId { get; }

    /// <summary>
    ///     Gets the language identifier.
    /// </summary>
    public int LanguageId { get; }

    /// <summary>
    ///     Gets the theme identifier.
    /// </summary>
    public int ThemeId { get; }
}
