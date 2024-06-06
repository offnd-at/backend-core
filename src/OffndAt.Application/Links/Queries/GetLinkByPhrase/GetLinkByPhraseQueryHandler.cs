namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

using Contracts.Links;
using Core.Abstractions.Messaging;
using Domain.Core.Primitives;
using Domain.Repositories;
using Domain.ValueObjects;

/// <summary>
///     Represents the <see cref="GetLinkByPhraseQuery" /> handler.
/// </summary>
internal sealed class GetLinkByPhraseQueryHandler(ILinksRepository linksRepository)
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

        return maybeLink.HasNoValue
            ? Maybe<GetLinkByPhraseResponse>.None
            : Maybe<GetLinkByPhraseResponse>.From(
                new GetLinkByPhraseResponse(new LinkDto(maybeLink.Value.TargetUrl, maybeLink.Value.Visits)));
    }
}
