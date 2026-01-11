using Microsoft.EntityFrameworkCore;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Persistence.Specifications;

namespace OffndAt.Persistence.Repositories;

/// <summary>
///     Represents the generic repository with the most common repository methods.
/// </summary>
/// <param name="dbContext">The database context.</param>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class GenericRepository<TEntity, TEntityId>(IDbContext dbContext)
    where TEntity : Entity<TEntityId>
    where TEntityId : EntityId
{
    /// <summary>
    ///     Gets the database context.
    /// </summary>
    protected IDbContext DbContext { get; } = dbContext;

    /// <summary>
    ///     Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the entity with the specified identifier.</returns>
    public async Task<Maybe<TEntity>> GetByIdAsync(TEntityId id, CancellationToken cancellationToken = default) =>
        await DbContext.Set<TEntity, TEntityId>()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

    /// <summary>
    ///     Gets the entities with the specified identifiers.
    /// </summary>
    /// <param name="ids">The entity identifiers.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the entities with the specified identifiers.</returns>
    public async Task<Maybe<IReadOnlyList<TEntity>>> GetManyByIdsAsync(
        IEnumerable<TEntityId> ids,
        CancellationToken cancellationToken = default) =>
        await DbContext.Set<TEntity, TEntityId>()
            .Where(entity => ids.Contains(entity.Id))
            .ToListAsync(cancellationToken);

    /// <summary>
    ///     Inserts the specified entity into the database.
    /// </summary>
    /// <param name="entity">The entity to be inserted into the database.</param>
    public void Insert(TEntity entity) => DbContext.Set<TEntity, TEntityId>().Add(entity);

    /// <summary>
    ///     Inserts the specified entities into the database.
    /// </summary>
    /// <param name="entities">The entities to be inserted into the database.</param>
    public void InsertRange(IEnumerable<TEntity> entities) => DbContext.Set<TEntity, TEntityId>().AddRange(entities);

    /// <summary>
    ///     Updates the specified entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    public void Update(TEntity entity) => DbContext.Set<TEntity, TEntityId>().Update(entity);

    /// <summary>
    ///     Removes the specified entity from the database.
    /// </summary>
    /// <param name="entity">The entity to be removed from the database.</param>
    public void Remove(TEntity entity) => DbContext.Set<TEntity, TEntityId>().Remove(entity);

    /// <summary>
    ///     Checks if any entity meets the specified specification.
    /// </summary>
    /// <param name="specification">The specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if any entity meets the specified specification, otherwise false.</returns>
    protected async Task<bool> AnyAsync(Specification<TEntity, TEntityId> specification, CancellationToken cancellationToken = default) =>
        await DbContext.Set<TEntity, TEntityId>().AnyAsync(specification, cancellationToken);

    /// <summary>
    ///     Gets the first entity that meets the specified specification.
    /// </summary>
    /// <param name="specification">The specification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the first entity that meets the specified specification.</returns>
    protected async Task<Maybe<TEntity>> FirstOrDefaultAsync(
        Specification<TEntity, TEntityId> specification,
        CancellationToken cancellationToken = default) =>
        await DbContext.Set<TEntity, TEntityId>().FirstOrDefaultAsync(specification, cancellationToken);
}
