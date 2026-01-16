using Microsoft.Extensions.Logging;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Abstractions.Telemetry;
using OffndAt.Domain.Abstractions.Services;
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
/// <param name="linkService">The link service.</param>
/// <param name="linkMetrics">The link metrics.</param>
/// <param name="logger">The logger.</param>
internal sealed class VisitLinkCommandHandler(
    ILinkRepository linkRepository,
    ILinkCache linkCache,
    ILinkService linkService,
    ILinkMetrics linkMetrics,
    ILogger<VisitLinkCommandHandler> logger)
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

        var maybeCachedLink = await linkCache.GetLinkAsync(phraseResult.Value, cancellationToken);
        if (maybeCachedLink.HasValue)
        {
            linkMetrics.RecordRedirectCacheHit();

            await linkService.RecordLinkVisitAsync(
                maybeCachedLink.Value.LinkId,
                new LinkVisitedContext(
                    maybeCachedLink.Value.Language,
                    maybeCachedLink.Value.Theme,
                    DateTimeOffset.UtcNow));

            return maybeCachedLink.Value.TargetUrl;
        }

        var maybeLink = await linkRepository.GetByPhraseAsync(phraseResult.Value, cancellationToken);
        if (maybeLink.HasNoValue)
        {
            return Result.Failure<Url>(DomainErrors.Link.NotFound);
        }

        await linkCache.SetLinkAsync(maybeLink.Value, cancellationToken);
        linkMetrics.RecordRedirectCacheMiss();
        maybeLink.Value.RecordVisit();

        return maybeLink.Value.TargetUrl;
    }
}
