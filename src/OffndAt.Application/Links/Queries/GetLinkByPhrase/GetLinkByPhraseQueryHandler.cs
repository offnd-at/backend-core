using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Links.Dtos;
using OffndAt.Contracts.Links.Responses;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

/// <summary>
///     Represents the <see cref="GetLinkByPhraseQuery" /> handler.
/// </summary>
internal sealed class GetLinkByPhraseQueryHandler(ILinksRepository linksRepository, IUnitOfWork unitOfWork)
    : IQueryHandler<GetLinkByPhraseQuery, GetLinkByPhraseResponse>
{
    /// <inheritdoc />
    public async Task<Maybe<GetLinkByPhraseResponse>> Handle(GetLinkByPhraseQuery request, CancellationToken cancellationToken)
    {
        var phraseResult = Phrase.Create(request.Phrase);
        if (phraseResult.IsFailure)
        {
            return Maybe<GetLinkByPhraseResponse>.None;
        }

        var maybeLink = await linksRepository.GetByPhraseAsync(phraseResult.Value, cancellationToken);

        if (maybeLink.HasNoValue)
        {
            return Maybe<GetLinkByPhraseResponse>.None;
        }

        if (request.ShouldIncrementVisits)
        {
            maybeLink.Value.IncrementVisits();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new GetLinkByPhraseResponse
        {
            Link = new LinkDto
            {
                Visits = maybeLink.Value.Visits,
                TargetUrl = maybeLink.Value.TargetUrl
            }
        };
    }
}
