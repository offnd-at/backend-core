using OffndAt.Application.IntegrationTests.Abstractions;
using OffndAt.Application.Links.Models;
using OffndAt.Application.Links.QueryServices;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.IntegrationTests.Links.QueryServices;

internal sealed class LinkQueryServiceTests : BaseIntegrationTest
{
    [Test]
    public async Task GetByPhraseAsync_ShouldReturnCorrectReadModel_IncludingSummaryAndRecentVisits() =>
        await ExecuteInTransactionAsync(async dbContext =>
        {
            var phrase = Phrase.Create("query-test-phrase").Value;
            var link = Link.Create(
                phrase,
                Url.Create("https://target.com").Value,
                Language.English,
                Theme.None);

            dbContext.Set<Link, LinkId>().Add(link);

            var summary = new LinkVisitSummary
            {
                LinkId = link.Id,
                TotalVisits = 123
            };

            dbContext.Set<LinkVisitSummary>().Add(summary);

            var visit1 = new LinkVisitLogEntry
            {
                Id = 1,
                LinkId = link.Id,
                VisitedAt = DateTimeOffset.UtcNow.AddMinutes(-10),
                IpAddress = "1.1.1.1",
                UserAgent = "Agent1",
                Referrer = "Ref1"
            };
            var visit2 = new LinkVisitLogEntry
            {
                Id = 2,
                LinkId = link.Id,
                VisitedAt = DateTimeOffset.UtcNow,
                IpAddress = "2.2.2.2",
                UserAgent = "Agent2",
                Referrer = "Ref2"
            };

            dbContext.Set<LinkVisitLogEntry>().AddRange(visit1, visit2);
            await dbContext.SaveChangesAsync();

            var queryService = new LinkQueryService(dbContext);
            var result = await queryService.GetByPhraseAsync(phrase, TestContext.CurrentContext.CancellationToken);

            Assert.Multiple(() =>
            {
                Assert.That(result.HasValue, Is.True);
                Assert.That(result.Value.Phrase, Is.EqualTo(phrase.Value));
                Assert.That(result.Value.VisitSummary.TotalVisits, Is.EqualTo(123));
                Assert.That(result.Value.RecentEntries, Has.Count.EqualTo(2));
                Assert.That(result.Value.RecentEntries.First().IpAddress, Is.EqualTo("2.2.2.2"));
                Assert.That(result.Value.RecentEntries.Skip(1).First().IpAddress, Is.EqualTo("1.1.1.1"));
            });
        });

    [Test]
    public async Task GetByPhraseAsync_ShouldReturnNone_WhenPhraseDoesNotExist() =>
        await ExecuteInTransactionAsync(async dbContext =>
        {
            var phrase = Phrase.Create("non-existent").Value;
            var queryService = new LinkQueryService(dbContext);

            var result = await queryService.GetByPhraseAsync(phrase, TestContext.CurrentContext.CancellationToken);

            Assert.That(result.HasNoValue, Is.True);
        });
}
