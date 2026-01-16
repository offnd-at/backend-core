using Microsoft.EntityFrameworkCore;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Links.Models;
using OffndAt.Application.Links.ReadModels;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Links.QueryServices;

/// <summary>
///     Provides functionality for querying information related to links.
/// </summary>
/// <param name="dbContext">The database context.</param>
internal sealed class LinkQueryService(IDbContext dbContext) : ILinkQueryService
{
    /// <inheritdoc />
    public async Task<Maybe<LinkReadModel>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default)
    {
        var linkAndSummaryQuery = from link in dbContext.Set<Link, LinkId>().AsNoTracking()
            where link.Phrase == phrase
            join visitSummary in dbContext.Set<LinkVisitSummary>().AsNoTracking()
                on link.Id equals visitSummary.LinkId into summaryGroup
            from visitSummary in summaryGroup.DefaultIfEmpty()
            let summary = visitSummary ??
                new LinkVisitSummary
                {
                    LinkId = link.Id,
                    TotalVisits = 0
                }
            select new
            {
                Link = link,
                Summary = summary
            };

        var linkWithSummary = await linkAndSummaryQuery.FirstOrDefaultAsync(cancellationToken);
        if (linkWithSummary == null)
        {
            return Maybe<LinkReadModel>.None;
        }

        var recentVisits = await dbContext.Set<LinkVisitLogEntry>()
            .AsNoTracking()
            .Where(visit => visit.LinkId == linkWithSummary.Link.Id)
            .OrderByDescending(v => v.VisitedAt)
            .Take(10)
            .Select(visit => new LinkVisitLogEntryReadModel
            {
                Id = visit.Id,
                LinkId = visit.LinkId,
                VisitedAt = visit.VisitedAt,
                IpAddress = visit.IpAddress,
                UserAgent = visit.UserAgent,
                Referrer = visit.Referrer
            })
            .ToListAsync(cancellationToken);

        return new LinkReadModel
        {
            Id = linkWithSummary.Link.Id,
            Phrase = linkWithSummary.Link.Phrase,
            TargetUrl = linkWithSummary.Link.TargetUrl,
            LanguageId = linkWithSummary.Link.Language.Value,
            ThemeId = linkWithSummary.Link.Theme.Value,
            VisitSummary = new LinkVisitSummaryReadModel
            {
                LinkId = linkWithSummary.Summary.LinkId,
                TotalVisits = linkWithSummary.Summary.TotalVisits
            },
            RecentEntries = recentVisits,
            CreatedAt = linkWithSummary.Link.CreatedAt
        };
    }
}
