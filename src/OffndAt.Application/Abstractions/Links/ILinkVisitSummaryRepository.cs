using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Abstractions.Links;

/// <summary>
///     Represents the link visit summary repository interface.
/// </summary>
public interface ILinkVisitSummaryRepository
{
    /// <summary>
    ///     Updates the total visit count for multiple links in a single operation.
    ///     If a link does not exist in the underlying store, it will be inserted with the provided count.
    /// </summary>
    /// <param name="updates">A collection of tuples where each tuple representing summaries to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task UpsertTotalVisitsForManyAsync(IReadOnlyCollection<(LinkId, long)> updates, CancellationToken cancellationToken);
}
