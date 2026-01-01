using System.Reflection;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace OffndAt.Persistence.Data;

/// <summary>
///     Represents the offnd.at application database context.
/// </summary>
/// <param name="options">The database context options.</param>
/// <param name="mediator">The mediator.</param>
public sealed class OffndAtDbContext(DbContextOptions options, IMediator mediator) : BaseDbContext(options, mediator)
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
