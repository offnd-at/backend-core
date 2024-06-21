namespace OffndAt.Domain.Events;

using Core.Events;
using Entities;

/// <summary>
///     Represents a domain event that is raised when a link is visited.
/// </summary>
public sealed class LinkVisitedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkVisitedDomainEvent" /> class.
    /// </summary>
    /// <param name="link">The link.</param>
    internal LinkVisitedDomainEvent(Link link) => Link = link;

    /// <summary>
    ///     Gets the link.
    /// </summary>
    public Link Link { get; }
}
