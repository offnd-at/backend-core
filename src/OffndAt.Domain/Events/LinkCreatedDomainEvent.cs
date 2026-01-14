using OffndAt.Domain.Core.Events;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.Events;

/// <summary>
///     Represents a domain event raised when a link is created.
/// </summary>
public sealed class LinkCreatedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkCreatedDomainEvent" /> class.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    internal LinkCreatedDomainEvent(LinkId linkId, Language language, Theme theme)
    {
        LinkId = linkId;
        Language = language;
        Theme = theme;
    }

    /// <summary>
    ///     Gets the link identifier.
    /// </summary>
    public LinkId LinkId { get; }

    /// <summary>
    ///     Gets the language.
    /// </summary>
    public Language Language { get; }

    /// <summary>
    ///     Gets the theme.
    /// </summary>
    public Theme Theme { get; }
}
