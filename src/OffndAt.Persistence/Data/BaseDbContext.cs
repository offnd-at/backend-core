namespace OffndAt.Persistence.Data;

using Application.Core.Abstractions.Data;
using Domain.Core.Abstractions;
using Domain.Core.Primitives;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

/// <summary>
///     Represents the application database context.
/// </summary>
/// <param name="options">The database context options.</param>
/// <param name="mediator">The mediator.</param>
public abstract class BaseDbContext(DbContextOptions options, IMediator mediator)
    : DbContext(options), IDbContext, IUnitOfWork
{
    /// <inheritdoc />
    public DbSet<TEntity> Set<TEntity, TEntityId>()
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId =>
        base.Set<TEntity>();

    /// <inheritdoc />
    public async Task<Maybe<TEntity>> GetByIdAsync<TEntity, TEntityId>(TEntityId id, CancellationToken cancellationToken = default)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId =>
        id == Guid.Empty
            ? Maybe<TEntity>.None
            : Maybe<TEntity>.From(await Set<TEntity, TEntityId>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken));

    /// <inheritdoc />
    public void Insert<TEntity, TEntityId>(TEntity entity)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId =>
        Set<TEntity, TEntityId>().Add(entity);

    /// <inheritdoc />
    public void InsertRange<TEntity, TEntityId>(IEnumerable<TEntity> entities)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId =>
        Set<TEntity, TEntityId>().AddRange(entities);

    /// <inheritdoc />
    public void Remove<TEntity, TEntityId>(TEntity entity)
        where TEntity : Entity<TEntityId>
        where TEntityId : EntityId =>
        Set<TEntity, TEntityId>().Remove(entity);

    /// <inheritdoc />
    public Task<int> ExecuteSqlAsync(
        string sql,
        IEnumerable<SqlParameter> parameters,
        CancellationToken cancellationToken = default) =>
        Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);

    /// <summary>
    ///     Saves all of the pending changes in the unit of work.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities that have been saved.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;

        UpdateAuditableEntities(utcNow);
        UpdateSoftDeletableEntities(utcNow);

        await PublishDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Database.BeginTransactionAsync(cancellationToken);

    /// <summary>
    ///     Updates the entities implementing <see cref="utcNow" /> interface.
    /// </summary>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    private void UpdateAuditableEntities(DateTimeOffset utcNow)
    {
        foreach (var entityEntry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = utcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(nameof(IAuditableEntity.ModifiedAt)).CurrentValue = utcNow;
            }
        }
    }

    /// <summary>
    ///     Updates the entities implementing <see cref="ISoftDeletableEntity" /> interface.
    /// </summary>
    /// <param name="utcNow">The current date and time in UTC format.</param>
    private void UpdateSoftDeletableEntities(DateTimeOffset utcNow)
    {
        foreach (var entityEntry in ChangeTracker.Entries<ISoftDeletableEntity>())
        {
            if (entityEntry.State != EntityState.Deleted)
            {
                continue;
            }

            entityEntry.Property(nameof(ISoftDeletableEntity.DeletedAt)).CurrentValue = utcNow;

            entityEntry.Property(nameof(ISoftDeletableEntity.IsDeleted)).CurrentValue = true;

            entityEntry.State = EntityState.Modified;

            UpdateDeletedEntityEntryReferencesToUnchanged(entityEntry);
        }
    }

    /// <summary>
    ///     Updates the specified entity entry's referenced entries in the deleted state to the modified state.
    ///     This method is recursive.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    private static void UpdateDeletedEntityEntryReferencesToUnchanged(EntityEntry? entityEntry)
    {
        if (!entityEntry?.References.Any() ?? false)
        {
            return;
        }

        foreach (var referenceEntry in entityEntry?.References.Where(r => r.TargetEntry?.State == EntityState.Deleted) ?? [])
        {
            if (referenceEntry.TargetEntry is not null)
            {
                referenceEntry.TargetEntry.State = EntityState.Unchanged;
            }

            UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
        }
    }

    /// <summary>
    ///     Publishes and then clears all the domain events that exist within the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregateRoots = ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entityEntry => entityEntry.Entity.DomainEvents.Count != 0)
            .ToList();

        var domainEvents = aggregateRoots.SelectMany(entityEntry => entityEntry.Entity.DomainEvents).ToList();

        aggregateRoots.ForEach(entityEntry => entityEntry.Entity.ClearDomainEvents());

        var tasks = domainEvents.Select(domainEvent => mediator.Publish(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);
    }
}
