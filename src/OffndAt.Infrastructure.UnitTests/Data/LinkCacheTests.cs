using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;
using OffndAt.Infrastructure.Core.Cache.Settings;
using OffndAt.Infrastructure.Data;

namespace OffndAt.Infrastructure.UnitTests.Data;

internal sealed class LinkCacheTests
{
    private IOptions<CacheSettings> _cacheOptions = null!;
    private LinkCache _linkCache = null!;
    private IMemoryCache _memoryCache = null!;

    [SetUp]
    public void Setup()
    {
        _memoryCache = Substitute.For<IMemoryCache>();
        _cacheOptions = Substitute.For<IOptions<CacheSettings>>();
        _cacheOptions.Value.Returns(
            new CacheSettings
            {
                LongTtl = TimeSpan.FromHours(1),
                ShortTtl = TimeSpan.FromMinutes(1)
            });

        _linkCache = new LinkCache(_cacheOptions, _memoryCache);
    }

    [TearDown]
    public void TearDown() => _memoryCache.Dispose();

    [Test]
    public async Task GetLinkAsync_ShouldReturnFromCache_WhenKeyExists()
    {
        var phrase = Phrase.Create("test").Value;
        var cachedLink = new CachedLink(
            new LinkId(Guid.NewGuid()),
            Url.Create("https://ex.com").Value,
            Language.English,
            Theme.None);

        _memoryCache.TryGetValue(Arg.Any<object>(), out Arg.Any<object?>())
            .Returns(x =>
            {
                x[1] = cachedLink;
                return true;
            });

        var result = await _linkCache.GetLinkAsync(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(cachedLink));
        });
    }

    [Test]
    public async Task SetLinkAsync_ShouldSetCacheWithCorrectKey()
    {
        var phrase = Phrase.Create("test").Value;
        var link = Link.Create(
            phrase,
            Url.Create("https://ex.com").Value,
            Language.English,
            Theme.None);

        await _linkCache.SetLinkAsync(link);

        _memoryCache.Received(1).CreateEntry(Arg.Any<object>());
    }
}
