using Microsoft.Extensions.Options;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Core.Settings;
using OffndAt.Infrastructure.Urls;

namespace OffndAt.Infrastructure.UnitTests.Urls;

internal sealed class UrlMakerTests
{
    [Test]
    public void MakeRedirectUrlForPhrase_ShouldUseHttps_WhenUseHttpsIsSetToTrueInApplicationSettings()
    {
        var settings = new ApplicationSettings
        {
            BaseDomain = "offnd.at",
            UseHttps = true,
            ApplicationName = "offnd-at",
            Environment = "test"
        };

        var phrase = Phrase.Create("test-phrase").Value;

        var urlMaker = new UrlMaker(Options.Create(settings));

        var actual = urlMaker.MakeRedirectUrlForPhrase(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value.Value, Contains.Substring("https://"));
        });
    }

    [Test]
    public void MakeRedirectUrlForPhrase_ShouldUseHttp_WhenUseHttpsIsSetToFalseInApplicationSettings()
    {
        var settings = new ApplicationSettings
        {
            BaseDomain = "offnd.at",
            UseHttps = false,
            ApplicationName = "offnd-at",
            Environment = "test"
        };

        var phrase = Phrase.Create("test-phrase").Value;

        var urlMaker = new UrlMaker(Options.Create(settings));

        var actual = urlMaker.MakeRedirectUrlForPhrase(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value.Value, Contains.Substring("http://"));
        });
    }

    [Test]
    public void MakeRedirectUrlForPhrase_ShouldBuiltRedirectUrlBasedOnApplicationSettingsAndProvidedPhrase()
    {
        var settings = new ApplicationSettings
        {
            BaseDomain = "offnd.at",
            UseHttps = true,
            ApplicationName = "offnd-at",
            Environment = "test"
        };

        var phrase = Phrase.Create("test-phrase").Value;
        var expectedUrl = Url.Create($"https://offnd.at/{phrase}").Value;

        var urlMaker = new UrlMaker(Options.Create(settings));

        var actual = urlMaker.MakeRedirectUrlForPhrase(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value, Is.EqualTo(expectedUrl));
        });
    }

    [Test]
    public void MakeStatsUrlForPhrase_ShouldUseHttps_WhenUseHttpsIsSetToTrueInApplicationSettings()
    {
        var settings = new ApplicationSettings
        {
            BaseDomain = "offnd.at",
            UseHttps = true,
            ApplicationName = "offnd-at",
            Environment = "test"
        };

        var phrase = Phrase.Create("test-phrase").Value;

        var urlMaker = new UrlMaker(Options.Create(settings));

        var actual = urlMaker.MakeStatsUrlForPhrase(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value.Value, Contains.Substring("https://"));
        });
    }

    [Test]
    public void MakeStatsUrlForPhrase_ShouldUseHttp_WhenUseHttpsIsSetToFalseInApplicationSettings()
    {
        var settings = new ApplicationSettings
        {
            BaseDomain = "offnd.at",
            UseHttps = false,
            ApplicationName = "offnd-at",
            Environment = "test"
        };

        var phrase = Phrase.Create("test-phrase").Value;

        var urlMaker = new UrlMaker(Options.Create(settings));

        var actual = urlMaker.MakeStatsUrlForPhrase(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value.Value, Contains.Substring("http://"));
        });
    }

    [Test]
    public void MakeStatsUrlForPhrase_ShouldBuiltStatsUrlBasedOnApplicationSettingsAndProvidedPhrase()
    {
        var settings = new ApplicationSettings
        {
            BaseDomain = "offnd.at",
            UseHttps = true,
            ApplicationName = "offnd-at",
            Environment = "test"
        };

        var phrase = Phrase.Create("test-phrase").Value;
        var expectedUrl = Url.Create($"https://offnd.at/s/{phrase}").Value;

        var urlMaker = new UrlMaker(Options.Create(settings));

        var actual = urlMaker.MakeStatsUrlForPhrase(phrase);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value, Is.EqualTo(expectedUrl));
        });
    }
}
