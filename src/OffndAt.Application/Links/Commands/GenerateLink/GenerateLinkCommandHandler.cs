using Microsoft.Extensions.Logging;
using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Application.Core.Abstractions.Phrases;
using OffndAt.Application.Core.Abstractions.Urls;
using OffndAt.Application.Core.Constants;
using OffndAt.Contracts.Links;
using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;
using Polly.Registry;

namespace OffndAt.Application.Links.Commands.GenerateLink;

/// <summary>
///     Handles the GenerateLinkCommand to create new shortened links.
/// </summary>
internal sealed class GenerateLinkCommandHandler(
    ILinksRepository linksRepository,
    IPhraseGenerator phraseGenerator,
    IUrlMaker urlMaker,
    ResiliencePipelineProvider<string> resiliencePipelineProvider,
    ILogger<GenerateLinkCommandHandler> logger)
    : ICommandHandler<GenerateLinkCommand, GenerateLinkResponse>
{
    /// <inheritdoc />
    public async Task<Result<GenerateLinkResponse>> Handle(GenerateLinkCommand request, CancellationToken cancellationToken)
    {
        var targetUrlResult = Url.Create(request.TargetUrl);
        if (targetUrlResult.IsFailure)
        {
            return Result.Failure<GenerateLinkResponse>(targetUrlResult.Error);
        }

        var maybeFormat = Format.FromValue(request.FormatId);
        if (maybeFormat.HasNoValue)
        {
            return Result.Failure<GenerateLinkResponse>(DomainErrors.Format.NotFound);
        }

        var maybeLanguage = Language.FromValue(request.LanguageId);
        if (maybeLanguage.HasNoValue)
        {
            return Result.Failure<GenerateLinkResponse>(DomainErrors.Language.NotFound);
        }

        var maybeTheme = Theme.FromValue(request.ThemeId);
        if (maybeTheme.HasNoValue)
        {
            return Result.Failure<GenerateLinkResponse>(DomainErrors.Theme.NotFound);
        }

        var retryPipeline = resiliencePipelineProvider.GetPipeline<Result<Phrase>>(ResiliencePolicies.PhraseGeneratorRetryPolicyName);

        var phraseResult = await retryPipeline.ExecuteAsync(
            async token =>
            {
                var phraseResult = await phraseGenerator.GenerateAsync(
                    maybeFormat.Value,
                    maybeLanguage.Value,
                    maybeTheme.Value,
                    token);

                if (phraseResult.IsFailure)
                {
                    return phraseResult;
                }

                if (await linksRepository.HasAnyWithPhraseAsync(phraseResult.Value, token))
                {
                    logger.LogWarning("The generated phrase is already in use := {Phrase}, retrying...", phraseResult.Value);

                    return Result.Failure<Phrase>(DomainErrors.Phrase.AlreadyInUse);
                }

                return phraseResult;
            },
            cancellationToken);

        if (phraseResult.IsFailure)
        {
            return Result.Failure<GenerateLinkResponse>(phraseResult.Error);
        }

        var link = Link.Create(
            phraseResult.Value,
            targetUrlResult.Value,
            maybeLanguage,
            maybeTheme);

        linksRepository.Insert(link);

        var redirectUrl = urlMaker.MakeRedirectUrlForPhrase(phraseResult.Value);
        var statsUrl = urlMaker.MakeStatsUrlForPhrase(phraseResult.Value);
        var firstFailureOrSuccess = Result.FirstFailureOrSuccess(redirectUrl, statsUrl);

        return firstFailureOrSuccess.IsFailure
            ? Result.Failure<GenerateLinkResponse>(firstFailureOrSuccess.Error)
            : Result.Success(
                new GenerateLinkResponse
                {
                    Url = redirectUrl.Value,
                    StatsUrl = statsUrl.Value
                });
    }
}
