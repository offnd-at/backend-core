using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
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
    public Task<Maybe<Url>> GetTargetUrlAsync(Phrase phrase, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.LinkTargetUrl(phrase);

        memoryCache.TryGetValue<Url>(cacheKey, out var url);

        return Task.FromResult(Maybe<Url>.From(url));
    }

    /// <inheritdoc />
    public Task SetTargetUrlAsync(Phrase phrase, Url targetUrl, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.LinkTargetUrl(phrase);

        memoryCache.Set(
            cacheKey,
            targetUrl,
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _settings.LongTtl,
                Priority = CacheItemPriority.High
            });

        return Task.CompletedTask;
    }
}
