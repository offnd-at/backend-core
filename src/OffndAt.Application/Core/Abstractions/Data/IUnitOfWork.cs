namespace OffndAt.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore.Storage;

/// <summary>
///     Defines the contract for managing database transactions and change tracking.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Saves all of the pending changes in the unit of work.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities that have been saved.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Begins a transaction on the current unit of work.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The new database context transaction.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
