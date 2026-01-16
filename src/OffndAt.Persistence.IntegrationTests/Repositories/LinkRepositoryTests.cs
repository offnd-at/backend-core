using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;
using OffndAt.Persistence.IntegrationTests.Abstractions;
using OffndAt.Persistence.Repositories;

namespace OffndAt.Persistence.IntegrationTests.Repositories;

internal sealed class LinkRepositoryTests : BaseIntegrationTest
{
    [Test]
    public async Task GetByPhraseAsync_ShouldReturnMaybeWithValue_WhenLinkWithSpecifiedPhraseExists() =>
        await ExecuteInTransactionAsync(async dbContext =>
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

            dbContext.Set<Link, LinkId>().Add(link);
            await dbContext.SaveChangesAsync();

            var repository = new LinkRepository(dbContext);
            var actual = await repository.GetByPhraseAsync(phrase, TestContext.CurrentContext.CancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(actual.Value, Is.EqualTo(link));
            });
        });

    [Test]
    public async Task GetByPhraseAsync_ShouldReturnEmptyMaybe_WhenLinkWithSpecifiedPhraseDoesNotExist() =>
        await ExecuteInTransactionAsync(async dbContext =>
        {
            var phrase = Phrase.Create("test-phrase").Value;

            var repository = new LinkRepository(dbContext);
            var actual = await repository.GetByPhraseAsync(phrase, TestContext.CurrentContext.CancellationToken);

            Assert.That(actual.HasValue, Is.False);
        });

    [Test]
    public async Task HasAnyWithPhraseAsync_ShouldReturnTrue_WhenLinkWithSpecifiedPhraseExists() =>
        await ExecuteInTransactionAsync(async dbContext =>
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

            dbContext.Set<Link, LinkId>().Add(link);
            await dbContext.SaveChangesAsync();

            var repository = new LinkRepository(dbContext);
            var actual = await repository.HasAnyWithPhraseAsync(phrase, TestContext.CurrentContext.CancellationToken);

            Assert.That(actual, Is.True);
        });

    [Test]
    public async Task HasAnyWithPhraseAsync_ShouldReturnFalse_WhenLinkWithSpecifiedPhraseDoesNotExist() =>
        await ExecuteInTransactionAsync(async dbContext =>
        {
            var phrase = Phrase.Create("test-phrase").Value;

            var repository = new LinkRepository(dbContext);
            var actual = await repository.HasAnyWithPhraseAsync(phrase, TestContext.CurrentContext.CancellationToken);

            Assert.That(actual, Is.False);
        });
}
