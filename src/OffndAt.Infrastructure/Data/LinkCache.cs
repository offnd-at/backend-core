using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Core.Cache.Settings;
using OffndAt.Infrastructure.Core.Constants;

namespace OffndAt.Infrastructure.Data;

/// <summary>
///     Provides caching for mappings between Links and their target URLs.
/// </summary>
/// <param name="cacheOptions">The cache options.</param>
/// <param name="memoryCache">The memory cache.</param>
internal sealed class LinkCache(IOptions<CacheSettings> cacheOptions, IMemoryCache memoryCache) : ILinkCache
{
    private readonly CacheSettings _settings = cacheOptions.Value;

    /// <inheritdoc />
    public Task<Maybe<CachedLink>> GetLinkAsync(Phrase phrase, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.LinkTargetUrl(phrase);

        memoryCache.TryGetValue<CachedLink>(cacheKey, out var url);

        return Task.FromResult(Maybe<CachedLink>.From(url));
    }

    /// <inheritdoc />
    public Task SetLinkAsync(Link link, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.LinkTargetUrl(link.Phrase);

        memoryCache.Set(
            cacheKey,
            new CachedLink(
                link.Id,
                link.TargetUrl,
                link.Language,
                link.Theme),
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _settings.LongTtl,
                Priority = CacheItemPriority.High
            });

        return Task.CompletedTask;
    }
}
