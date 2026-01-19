using Microsoft.Extensions.Logging;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Links.Dtos;
using OffndAt.Contracts.Links.Responses;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

/// <summary>
///     Represents the <see cref="GetLinkByPhraseQuery" /> handler.
/// </summary>
/// <param name="queryService">The link query service.</param>
/// <param name="logger">The logger.</param>
internal sealed class GetLinkByPhraseQueryHandler(ILinkQueryService queryService, ILogger<GetLinkByPhraseQueryHandler> logger)
    : IQueryHandler<GetLinkByPhraseQuery, GetLinkByPhraseResponse>
{
    /// <inheritdoc />
    public async Task<Maybe<GetLinkByPhraseResponse>> Handle(GetLinkByPhraseQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching a link with phrase := {Phrase}", request.Phrase);

        var phraseResult = Phrase.Create(request.Phrase);
        if (phraseResult.IsFailure)
        {
            return Maybe<GetLinkByPhraseResponse>.None;
        }

        var maybeLink = await queryService.GetByPhraseAsync(phraseResult.Value, cancellationToken);
        if (maybeLink.HasNoValue)
        {
            return Maybe<GetLinkByPhraseResponse>.None;
        }

        return new GetLinkByPhraseResponse
        {
            Id = maybeLink.Value.Id,
            Phrase = maybeLink.Value.Phrase,
            TargetUrl = maybeLink.Value.TargetUrl,
            LanguageId = maybeLink.Value.LanguageId,
            ThemeId = maybeLink.Value.ThemeId,
            Visits = maybeLink.Value.VisitSummary.TotalVisits,
            RecentVisits = maybeLink.Value.RecentEntries.Select(entry => new LinkVisitDto
            {
                VisitedAt = entry.VisitedAt,
                IpAddress = entry.IpAddress,
                UserAgent = entry.UserAgent,
                Referrer = entry.Referrer
            }),
            CreatedAt = maybeLink.Value.CreatedAt
        };
    }
}
