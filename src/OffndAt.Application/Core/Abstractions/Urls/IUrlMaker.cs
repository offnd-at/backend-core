namespace OffndAt.Application.Core.Abstractions.Urls;

using Domain.Core.Primitives;
using Domain.ValueObjects;

/// <summary>
///     Defines the contract for constructing and validating URLs.
/// </summary>
public interface IUrlMaker
{
    /// <summary>
    ///     Creates a redirect <see cref="Url" /> based on the specified <see cref="Phrase" />.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <returns>The result of the URL creation process containing the <see cref="Url" /> or an <see cref="Error" />.</returns>
    Result<Url> MakeRedirectUrlForPhrase(Phrase phrase);

    /// <summary>
    ///     Creates a stats <see cref="Url" /> based on the specified <see cref="Phrase" />.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <returns>The result of the URL creation process containing the <see cref="Url" /> or an <see cref="Error" />.</returns>
    Result<Url> MakeStatsUrlForPhrase(Phrase phrase);
}
