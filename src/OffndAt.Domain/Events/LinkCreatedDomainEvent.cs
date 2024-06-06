namespace OffndAt.Domain.Events;

using Core.Events;
using Entities;

/// <summary>
///     Represents a domain event that is raised when a link is created.
/// </summary>
public sealed class LinkCreatedDomainEvent : IDomainEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkCreatedDomainEvent" /> class.
    /// </summary>
    /// <param name="link">The link.</param>
    internal LinkCreatedDomainEvent(Link link) => Link = link;

    /// <summary>
    ///     Gets the link.
    /// </summary>
    public Link Link { get; }
}
