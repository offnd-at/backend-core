using OffndAt.Application.Links.Models;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;
using OffndAt.Persistence.IntegrationTests.Abstractions;
using OffndAt.Persistence.Repositories;

namespace OffndAt.Persistence.IntegrationTests.Repositories;

internal sealed class LinkVisitSummaryRepositoryTests : BaseIntegrationTest
{
    [Test]
    public async Task UpsertTotalVisitsForManyAsync_ShouldInsertNewSummaries_WhenTheyDoNotExist() =>
        await ExecuteInTransactionAsync(async dbContext =>
        {
            var phrase = Phrase.Create("test-phrase-1").Value;
            var link = Link.Create(
                phrase,
                Url.Create("https://example.com").Value,
                Language.English,
                Theme.None);
            dbContext.Set<Link, LinkId>().Add(link);
            await dbContext.SaveChangesAsync();

            var repository = new LinkVisitSummaryRepository(dbContext);
            var updates = new List<(LinkId, long)>
            {
                (link.Id, 5)
            };

            await repository.UpsertTotalVisitsForManyAsync(updates, TestContext.CurrentContext.CancellationToken);

            var summary = await dbContext.Set<LinkVisitSummary>().FindAsync(link.Id);
            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.TotalVisits, Is.EqualTo(5));
        });

    [Test]
    public async Task UpsertTotalVisitsForManyAsync_ShouldUpdateExistingSummaries_WhenTheyAlreadyExist() =>
        await ExecuteInTransactionAsync(async dbContext =>
        {
            var phrase = Phrase.Create("test-phrase-2").Value;
            var link = Link.Create(
                phrase,
                Url.Create("https://example.com").Value,
                Language.English,
                Theme.None);
            dbContext.Set<Link, LinkId>().Add(link);

            var existingSummary = new LinkVisitSummary
            {
                LinkId = link.Id,
                TotalVisits = 10
            };
            dbContext.Set<LinkVisitSummary>().Add(existingSummary);
            await dbContext.SaveChangesAsync();

            var repository = new LinkVisitSummaryRepository(dbContext);
            var updates = new List<(LinkId, long)>
            {
                (link.Id, 5)
            };

            await repository.UpsertTotalVisitsForManyAsync(updates, TestContext.CurrentContext.CancellationToken);
            await dbContext.Entry(existingSummary).ReloadAsync();

            var summary = await dbContext.Set<LinkVisitSummary>().FindAsync(link.Id);
            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.TotalVisits, Is.EqualTo(15));
        });
}
