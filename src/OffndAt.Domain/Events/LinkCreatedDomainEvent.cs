using OffndAt.Domain.Core.Events;
using OffndAt.Domain.Entities;

namespace OffndAt.Domain.Events;

/// <summary>
///     Domain event published when a new shortened link is created.
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
