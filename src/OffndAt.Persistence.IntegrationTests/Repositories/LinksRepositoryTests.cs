﻿namespace OffndAt.Persistence.IntegrationTests.Repositories;

using Domain.Entities;
using Domain.Enumerations;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;

internal sealed class LinksRepositoryTests : BaseTestFixture
{
    private LinksRepository _repository = null!;

    [SetUp]
    public void Setup() => _repository = new LinksRepository(DbContext);

    [TearDown]
    public async Task Teardown() => await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM \"Link\"");

    [Test]
    public async Task GetByPhraseAsync_ShouldReturnMaybeWithValue_WhenLinkWithSpecifiedPhraseExists()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var language = Language.English;
        var theme = Theme.None;

        var link = Link.Create(
            phrase,
            targetUrl,
            language,
            theme);

        DbContext.Set<Link, LinkId>().Add(link);
        await DbContext.SaveChangesAsync();

        var actual = await _repository.GetByPhraseAsync(phrase, CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(link));
            });
    }

    [Test]
    public async Task GetByPhraseAsync_ShouldReturnEmptyMaybe_WhenLinkWithSpecifiedPhraseDoesNotExist()
    {
        var phrase = Phrase.Create("test-phrase").Value;

        var actual = await _repository.GetByPhraseAsync(phrase, CancellationToken.None);

        Assert.That(actual.HasValue, Is.False);
    }

    [Test]
    public async Task HasAnyWithPhraseAsync_ShouldReturnTrue_WhenLinkWithSpecifiedPhraseExists()
    {
        var phrase = Phrase.Create("test-phrase").Value;
        var targetUrl = Url.Create("https://example.com").Value;
        var language = Language.English;
        var theme = Theme.None;

        var link = Link.Create(
            phrase,
            targetUrl,
            language,
            theme);

        DbContext.Set<Link, LinkId>().Add(link);
        await DbContext.SaveChangesAsync();

        var actual = await _repository.HasAnyWithPhraseAsync(phrase, CancellationToken.None);

        Assert.That(actual, Is.True);
    }

    [Test]
    public async Task HasAnyWithPhraseAsync_ShouldReturnFalse_WhenLinkWithSpecifiedPhraseDoesNotExist()
    {
        var phrase = Phrase.Create("test-phrase").Value;

        var actual = await _repository.HasAnyWithPhraseAsync(phrase, CancellationToken.None);

        Assert.That(actual, Is.False);
    }
}
