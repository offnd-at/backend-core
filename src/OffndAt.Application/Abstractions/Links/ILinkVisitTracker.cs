using OffndAt.Application.Links.Models;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Abstractions.Links;

/// <summary>
///     Provides a mechanism to track visits for links.
/// </summary>
public interface ILinkVisitTracker
{
    /// <summary>
    ///     Records that a visit occurred for the given link.
    /// </summary>
    /// <param name="linkId">The link identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task RecordAsync(LinkId linkId, CancellationToken cancellationToken);

    /// <summary>
    ///     Returns all accumulated visit counts and clears the store.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A read-only collection of <see cref="LinkTrackerItem" />.</returns>
    Task<IReadOnlyCollection<LinkTrackerItem>> DrainAsync(CancellationToken cancellationToken);
}
