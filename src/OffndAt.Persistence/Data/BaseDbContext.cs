using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Domain.Core.Abstractions;
using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Persistence.Data;

/// <summary>
///     Provides base Entity Framework Core database context functionality.
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
    public Task<int> ExecuteSqlAsync(
        string sql,
        IEnumerable<SqlParameter> parameters,
        CancellationToken cancellationToken = default) =>
        Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);

    /// <summary>
    ///     Saves all the pending changes in the unit of work.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities that have been saved.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTimeOffset.UtcNow;

        var shouldLoop = true;

        while (shouldLoop)
        {
            UpdateAuditableEntities(utcNow);
            UpdateSoftDeletableEntities(utcNow);

            shouldLoop = await PublishDomainEventsAsync(cancellationToken) > 0;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        Database.BeginTransactionAsync(cancellationToken);

    /// <summary>
    ///     Updates the entities implementing <see cref="IAuditableEntity" /> interface.
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
            referenceEntry.TargetEntry?.State = EntityState.Unchanged;

            UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
        }
    }

    /// <summary>
    ///     Publishes and then clears all the domain events that exist within the current transaction.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of domain events that have been published.</returns>
    private async Task<int> PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregateRoots = GetAggregateRootsWithDomainEvents();

        var domainEvents = aggregateRoots.SelectMany(entityEntry => entityEntry.Entity.DomainEvents).ToList();

        var tasks = domainEvents.Select(domainEvent => mediator.Publish(domainEvent, cancellationToken));

        await Task.WhenAll(tasks);

        aggregateRoots.ForEach(entityEntry => entityEntry.Entity.ClearDomainEvents());

        return domainEvents.Count;
    }

    /// <summary>
    ///     Gets the aggregate roots that raised at least one domain event.
    /// </summary>
    /// <returns>The list of aggregate roots.</returns>
    private List<EntityEntry<IAggregateRoot>> GetAggregateRootsWithDomainEvents() =>
        ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(entityEntry => entityEntry.Entity.DomainEvents.Count != 0)
            .ToList();
}
