using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Core.Abstractions.Phrases;

/// <summary>
///     Defines the contract for generating unique human-readable phrases.
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
