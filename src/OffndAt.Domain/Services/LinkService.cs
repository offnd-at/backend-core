using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Abstractions.Services;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.Services;

/// <summary>
///     Represents the link service.
/// </summary>
/// <param name="domainEventCollector">The domain event collector.</param>
internal sealed class LinkService(IDomainEventCollector domainEventCollector) : ILinkService
{
    /// <inheritdoc />
    public Task RecordLinkVisitAsync(LinkId id, LinkVisitedContext context)
    {
        domainEventCollector.RaiseEvent(new LinkVisitedDomainEvent(id, context));
        return Task.CompletedTask;
    }
}
