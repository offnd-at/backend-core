using System.Collections.Concurrent;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Infrastructure.Links;

/// <summary>
///     Implements a mechanism to track visits for links.
/// </summary>
internal sealed class LinkVisitTracker : ILinkVisitTracker
{
    private ConcurrentDictionary<LinkId, long> _counts = new();

    /// <inheritdoc />
    public Task RecordAsync(LinkId linkId, CancellationToken cancellationToken)
    {
        _counts.AddOrUpdate(linkId, _ => 1, (_, current) => current + 1);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<LinkTrackerItem>> DrainAsync(CancellationToken cancellationToken)
    {
        var old = Interlocked.Exchange(ref _counts, new ConcurrentDictionary<LinkId, long>());

        var snapshot = old
            .Select(kvp => new LinkTrackerItem(kvp.Key, kvp.Value))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<LinkTrackerItem>>(snapshot);
    }
}
