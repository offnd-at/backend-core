namespace OffndAt.Application.Core.Abstractions.Data;

using Domain.Core.Primitives;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
    ///     Gets the entity with the specified identifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TEntityId">The entity identifier type.</typeparam>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the <typeparamref name="TEntity" /> with the specified identifier if it exists.</returns>
    Task<Maybe<TEntity>> GetByIdAsync<TEntity, TEntityId>(TEntityId id, CancellationToken cancellationToken = default)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId;

    /// <summary>
    ///     Inserts the specified entity into the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TEntityId">The entity identifier type.</typeparam>
    /// <param name="entity">The entity to be inserted into the database.</param>
    void Insert<TEntity, TEntityId>(TEntity entity)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId;

    /// <summary>
    ///     Inserts the specified entities into the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TEntityId">The entity identifier type.</typeparam>
    /// <param name="entities">The entities to be inserted into the database.</param>
    void InsertRange<TEntity, TEntityId>(IEnumerable<TEntity> entities)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId;

    /// <summary>
    ///     Removes the specified entity from the database.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TEntityId">The entity identifier type.</typeparam>
    /// <param name="entity">The entity to be removed from the database.</param>
    void Remove<TEntity, TEntityId>(TEntity entity)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId;

    /// <summary>
    ///     Executes the specified SQL command asynchronously and returns the number of affected rows.
    /// </summary>
    /// <param name="sql">The SQL command.</param>
    /// <param name="parameters">The parameters collection.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecuteSqlAsync(
        string sql,
        IEnumerable<SqlParameter> parameters,
        CancellationToken cancellationToken = default);
}
