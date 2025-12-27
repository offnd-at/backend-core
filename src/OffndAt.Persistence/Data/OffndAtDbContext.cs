using System.Reflection;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace OffndAt.Persistence.Data;

/// <summary>
///     Provides Entity Framework Core database context for the offnd.at application.
/// </summary>
/// <param name="options">The database context options.</param>
/// <param name="mediator">The mediator.</param>
public sealed class OffndAtDbContext(DbContextOptions options, IMediator mediator) : BaseDbContext(options, mediator)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
    }
}
