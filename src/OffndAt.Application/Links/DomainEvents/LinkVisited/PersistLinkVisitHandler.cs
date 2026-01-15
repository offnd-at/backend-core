using Microsoft.AspNetCore.Http;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Application.Links.DomainEvents.LinkVisited;

/// <summary>
///     Handles the <see cref="LinkVisitedDomainEvent" /> by persisting visit details to the database.
/// </summary>
/// <param name="dbContext">The database context.</param>
/// <param name="httpContext">The HTTP context accessor.</param>
internal sealed class PersistLinkVisitHandler(IDbContext dbContext, IHttpContextAccessor httpContext)
    : IDomainEventHandler<LinkVisitedDomainEvent>
{
    /// <inheritdoc />
    public async Task Handle(LinkVisitedDomainEvent notification, CancellationToken cancellationToken) =>
        await dbContext.Set<LinkVisitLogEntry>()
            .AddAsync(
                new LinkVisitLogEntry
                {
                    Id = 0,
                    LinkId = notification.LinkId,
                    VisitedAt = notification.Context.VisitedAt,
                    IpAddress = httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext.HttpContext?.Request.Headers.UserAgent.ToString(),
                    Referrer = httpContext.HttpContext?.Request.Headers.Referer.ToString()
                },
                cancellationToken);
}
