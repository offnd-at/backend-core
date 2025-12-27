using Contracts.Links;
using Core.Abstractions.Data;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Repositories;
using Domain.ValueObjects;


namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;/// <summary>
///     Handles the GetLinkByPhraseQuery to retrieve links by their unique phrase.
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
