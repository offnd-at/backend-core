using OffndAt.Application.Links.Models;
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
    ///     Gets the cached link for a given <see cref="Phrase" />.
    /// </summary>
    /// <param name="phrase">The phrase associated with a link pointing to the target URL.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the <see cref="CachedLink" /> associated with the given <see cref="Phrase" />.</returns>
    Task<Maybe<CachedLink>> GetLinkAsync(Phrase phrase, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Caches the given link for the given <see cref="Phrase" />.
    /// </summary>
    /// <param name="link">The link.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The awaitable task.</returns>
    Task SetLinkAsync(Link link, CancellationToken cancellationToken = default);
}
