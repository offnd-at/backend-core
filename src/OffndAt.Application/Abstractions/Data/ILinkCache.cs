using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Abstractions.Data;

/// <summary>
///     Provides caching for mappings between Links and their target URLs.
/// </summary>
public interface ILinkCache
{
    /// <summary>
    ///     Gets the cached target URL for a given <see cref="Phrase" />.
    /// </summary>
    /// <param name="phrase">The phrase associated with a link pointing to the target URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the target URL of the <see cref="Link" /> associated with the given <see cref="Phrase" />.</returns>
    Task<Maybe<Url>> GetTargetUrlAsync(Phrase phrase, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Caches the given target URL for the given <see cref="Phrase" />.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <param name="targetUrl">The target URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The awaitable task.</returns>
    Task SetTargetUrlAsync(Phrase phrase, Url targetUrl, CancellationToken cancellationToken = default);
}
