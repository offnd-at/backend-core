namespace OffndAt.Application.Core.Abstractions.Phrases;

using Domain.Core.Primitives;
using Domain.Enumerations;
using Domain.ValueObjects;

/// <summary>
///     Represents the phrase generator interface.
/// </summary>
public interface IPhraseGenerator
{
    /// <summary>
    ///     Generates a <see cref="Phrase" /> using specified parameters.
    /// </summary>
    /// <param name="format">The casing format.</param>
    /// <param name="language">The language.</param>
    /// <param name="theme">The theme.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the phrase generation process containing the <see cref="Phrase" /> or an <see cref="Error" />.</returns>
    Task<Result<Phrase>> GenerateAsync(
        Format format,
        Language language,
        Theme theme,
        CancellationToken cancellationToken = default);
}
