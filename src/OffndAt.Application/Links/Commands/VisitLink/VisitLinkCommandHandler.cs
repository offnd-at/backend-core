using Microsoft.Extensions.Logging;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Links.Commands.VisitLink;

/// <summary>
///     Represents the <see cref="VisitLinkCommand" /> handler.
/// </summary>
/// <param name="linkRepository">The link repository.</param>
/// <param name="linkCache">The link cache.</param>
/// <param name="logger">The logger.</param>
internal sealed class VisitLinkCommandHandler(ILinkRepository linkRepository, ILinkCache linkCache, ILogger<VisitLinkCommandHandler> logger)
    : ICommandHandler<VisitLinkCommand, Url>
{
    /// <inheritdoc />
    public async Task<Result<Url>> Handle(VisitLinkCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to visit a link with phrase := {Phrase}", request.Phrase);

        var phraseResult = Phrase.Create(request.Phrase);
        if (phraseResult.IsFailure)
        {
            return Result.Failure<Url>(phraseResult.Error);
        }

        var maybeCachedUrl = await linkCache.GetTargetUrlAsync(phraseResult.Value, cancellationToken);
        if (maybeCachedUrl.HasValue)
        {
            // TODO: get value from cache here, create domain service for raising events and call it here
            // so that domain publishes event

            return maybeCachedUrl.Value;
        }

        var maybeLink = await linkRepository.GetByPhraseAsync(phraseResult.Value, cancellationToken);
        if (maybeLink.HasNoValue)
        {
            return Result.Failure<Url>(DomainErrors.Link.NotFound);
        }

        await linkCache.SetTargetUrlAsync(maybeLink.Value.Phrase, maybeLink.Value.TargetUrl, cancellationToken);
        maybeLink.Value.RecordVisit();

        return maybeLink.Value.TargetUrl;
    }
}
