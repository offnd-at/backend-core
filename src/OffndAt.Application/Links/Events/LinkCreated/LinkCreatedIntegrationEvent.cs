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

    [JsonConstructor]
    private LinkCreatedIntegrationEvent()
    {
    }

    /// <summary>
    ///     Gets the link identifier.
    /// </summary>
    public Guid LinkId { get; }
}
