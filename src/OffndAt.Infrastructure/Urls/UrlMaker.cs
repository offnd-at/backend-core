using Microsoft.Extensions.Options;
using OffndAt.Application.Core.Abstractions.Urls;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Core.Settings;

namespace OffndAt.Infrastructure.Urls;

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
