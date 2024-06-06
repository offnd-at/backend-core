namespace OffndAt.Persistence.Data;

using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;

/// <summary>
///     Represents the offnd.at application database context.
/// </summary>
/// <param name="options">The database context options.</param>
/// <param name="mediator">The mediator.</param>
internal sealed class OffndAtDbContext(DbContextOptions options, IMediator mediator) : BaseDbContext(options, mediator)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
