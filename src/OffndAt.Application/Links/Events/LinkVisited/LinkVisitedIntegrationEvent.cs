using System.Text.Json.Serialization;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.Events.LinkVisited;

/// <summary>
///     Represents an integration event raised when a link is visited.
/// </summary>
public sealed class LinkVisitedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkVisitedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="domainEvent">The link visited domain event.</param>
    internal LinkVisitedIntegrationEvent(LinkVisitedDomainEvent domainEvent) => LinkId = domainEvent.LinkId;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkVisitedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <remarks>
    ///     Required by JSON deserializer.
    /// </remarks>
    [JsonConstructor]
    private LinkVisitedIntegrationEvent(Guid linkId) => LinkId = linkId;

    /// <summary>
    ///     Gets the link identifier.
    /// </summary>
    public Guid LinkId { get; }
}
