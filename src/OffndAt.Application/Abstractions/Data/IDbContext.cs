using Microsoft.EntityFrameworkCore;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Application.Abstractions.Data;

/// <summary>
///     Represents the application database context interface.
/// </summary>
public interface IDbContext
{
    /// <summary>
    ///     Gets the database set for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TEntityId">The entity identifier type.</typeparam>
    /// <returns>The database set for the specified entity type.</returns>
    DbSet<TEntity> Set<TEntity, TEntityId>()
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId;

    /// <summary>
    ///     Gets the database set for the specified data model type.
    /// </summary>
    /// <typeparam name="TDataModel">The data model type.</typeparam>
    /// <returns>The database set for the specified data model type.</returns>
    DbSet<TDataModel> Set<TDataModel>()
        where TDataModel : class, INonEntityDataModel;

    /// <summary>
    ///     Executes the specified SQL command asynchronously and returns the number of affected rows.
    /// </summary>
    /// <param name="sql">The SQL command.</param>
    /// <param name="parameters">The parameters collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteSqlAsync(
        string sql,
        IEnumerable<object> parameters,
        CancellationToken cancellationToken = default);
}
