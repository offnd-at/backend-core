using Microsoft.Extensions.Options;
using OffndAt.Application.Abstractions.Urls;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Core.Settings;

namespace OffndAt.Infrastructure.Urls;

/// <summary>
///     Represents the URL maker.
/// </summary>
/// <param name="applicationOptions">The application options.</param>
internal sealed class UrlMaker(IOptions<ApplicationSettings> applicationOptions) : IUrlMaker
{
    private readonly ApplicationSettings _applicationSettings = applicationOptions.Value;

    private string Protocol =>
        _applicationSettings.UseHttps!.Value ? "https" : "http";

    private string BaseUrl =>
        $"{Protocol}://{_applicationSettings.BaseDomain!}";

    /// <inheritdoc />
    public Result<Url> MakeRedirectUrlForPhrase(Phrase phrase) => Url.Create($"{BaseUrl}/{phrase}");

    /// <inheritdoc />
    public Result<Url> MakeStatsUrlForPhrase(Phrase phrase) => Url.Create($"{BaseUrl}/s/{phrase}");
}
