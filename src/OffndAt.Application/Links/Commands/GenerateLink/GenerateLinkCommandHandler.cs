namespace OffndAt.Application.Links.Commands.GenerateLink;

using Contracts.Links;
using Core.Abstractions.Messaging;
using Core.Abstractions.Phrases;
using Core.Abstractions.Urls;
using Core.Constants;
using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.Entities;
using Domain.Enumerations;
using Domain.Repositories;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Polly.Registry;

/// <summary>
///     Represents the <see cref="GenerateLinkCommand" /> handler.
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

                if (await linksRepository.HasAnyWithPhraseAsync(phraseResult.Value, cancellationToken))
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
            : Result.Success(new GenerateLinkResponse(redirectUrl.Value, statsUrl.Value));
    }
}
