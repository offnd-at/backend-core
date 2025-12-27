namespace OffndAt.Domain.Events;

using Core.Events;
using Entities;

/// <summary>
///     Domain event published when a user accesses a shortened link.
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
