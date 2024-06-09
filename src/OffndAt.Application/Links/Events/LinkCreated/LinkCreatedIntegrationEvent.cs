namespace OffndAt.Application.Links.Events.LinkCreated;

using System.Text.Json.Serialization;
using Core.Abstractions.Messaging;
using Domain.Events;

/// <summary>
///     Represents an integration event that is raised when a link is created.
/// </summary>
public sealed class LinkCreatedIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkCreatedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="domainEvent">The link created domain event.</param>
    internal LinkCreatedIntegrationEvent(LinkCreatedDomainEvent domainEvent) => LinkId = domainEvent.Link.Id;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkCreatedIntegrationEvent" /> class.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <remarks>
    ///     Required by JSON deserializer.
    /// </remarks>
    [JsonConstructor]
    private LinkCreatedIntegrationEvent(Guid linkId) => LinkId = linkId;

    /// <summary>
    ///     Gets the link identifier.
    /// </summary>
    public Guid LinkId { get; }
}
