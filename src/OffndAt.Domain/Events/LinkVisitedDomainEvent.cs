using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.Events;

/// <summary>
///     Represents a domain event raised when a link is visited.
/// </summary>
public sealed class LinkVisitedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkVisitedDomainEvent" /> class.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <param name="context">The context of the link visit.</param>
    internal LinkVisitedDomainEvent(
        LinkId linkId,
        LinkVisitedContext context)
    {
        LinkId = linkId;
        Context = context;
    }

    /// <summary>
    ///     Gets the link identifier.
    /// </summary>
    public LinkId LinkId { get; }

    /// <summary>
    ///     Gets the context of the link visit.
    /// </summary>
    public LinkVisitedContext Context { get; }
}
