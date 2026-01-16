using System.Text.Json.Serialization;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.IntegrationEvents.LinkCreated;

/// <summary>
///     Represents an integration event raised when a link is created.
/// </summary>
public sealed class LinkCreatedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkCreatedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="domainEvent">The link created domain event.</param>
    internal LinkCreatedIntegrationEvent(LinkCreatedDomainEvent domainEvent)
    {
        LinkId = domainEvent.LinkId;
        LanguageId = domainEvent.Language.Value;
        ThemeId = domainEvent.Theme.Value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkCreatedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <param name="languageId">The language identifier.</param>
    /// <param name="themeId">The theme identifier.</param>
    /// <remarks>
    ///     Required by JSON deserializer.
    /// </remarks>
    [JsonConstructor]
    private LinkCreatedIntegrationEvent(Guid linkId, int languageId, int themeId)
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
