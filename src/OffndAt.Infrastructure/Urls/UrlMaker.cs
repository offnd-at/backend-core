namespace OffndAt.Infrastructure.Urls;

using Application.Core.Abstractions.Urls;
using Core.Settings;
using Domain.Core.Primitives;
using Domain.ValueObjects;
using Microsoft.Extensions.Options;

/// <summary>
///     Constructs and validates URLs for shortened links.
/// </summary>
/// <param name="applicationOptions">The application options.</param>
internal sealed class UrlMaker(IOptions<ApplicationSettings> applicationOptions) : IUrlMaker
{
    private readonly ApplicationSettings _applicationSettings = applicationOptions.Value;

    private string Protocol => _applicationSettings.UseHttps ? "https" : "http";

    private string BaseUrl => $"{Protocol}://{_applicationSettings.BaseDomain}";

    /// <inheritdoc />
    public Result<Url> MakeRedirectUrlForPhrase(Phrase phrase) => Url.Create($"{BaseUrl}/{phrase}");

    /// <inheritdoc />
    public Result<Url> MakeStatsUrlForPhrase(Phrase phrase) => Url.Create($"{BaseUrl}/s/{phrase}");
}
