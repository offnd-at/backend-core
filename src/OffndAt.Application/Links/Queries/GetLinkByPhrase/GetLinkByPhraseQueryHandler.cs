using Microsoft.Extensions.Logging;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Links.Dtos;
using OffndAt.Contracts.Links.Responses;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

/// <summary>
///     Represents the <see cref="GetLinkByPhraseQuery" /> handler.
/// </summary>
/// <param name="linkRepository">The link repository.</param>
/// <param name="logger">The logger.</param>
internal sealed class GetLinkByPhraseQueryHandler(ILinkRepository linkRepository, ILogger<GetLinkByPhraseQueryHandler> logger)
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

        var maybeLink = await linkRepository.GetByPhraseAsync(phraseResult.Value, cancellationToken);
        if (maybeLink.HasNoValue)
        {
            return Maybe<GetLinkByPhraseResponse>.None;
        }

        return new GetLinkByPhraseResponse
        {
            Link = new LinkDto
            {
                // TODO: query service
                Visits = 0,
                TargetUrl = maybeLink.Value.TargetUrl,
                CreatedAt = maybeLink.Value.CreatedAt
            }
        };
    }
}
