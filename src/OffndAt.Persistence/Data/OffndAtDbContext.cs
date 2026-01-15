using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OffndAt.Domain.Abstractions.Events;

namespace OffndAt.Persistence.Data;

/// <summary>
///     Represents the offnd.at application database context.
/// </summary>
/// <param name="options">The database context options.</param>
/// <param name="domainEventPublisher">The domain event publisher.</param>
/// <param name="domainEventCollector">The domain event collector.</param>
public sealed class OffndAtDbContext(
    DbContextOptions options,
    IDomainEventPublisher domainEventPublisher,
    IDomainEventCollector domainEventCollector)
    : BaseDbContext(options, domainEventPublisher, domainEventCollector)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
    }
}
