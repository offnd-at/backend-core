using OffndAt.Domain.ValueObjects.Identifiers;
using OffndAt.Infrastructure.Links;

namespace OffndAt.Infrastructure.UnitTests.Links;

internal sealed class LinkVisitTrackerTests
{
    private LinkVisitTracker _tracker = null!;

    [SetUp]
    public void Setup() => _tracker = new LinkVisitTracker();

    [Test]
    public async Task RecordAsync_ShouldIncrementCount()
    {
        var linkId = new LinkId(Guid.NewGuid());

        await _tracker.RecordAsync(linkId, TestContext.CurrentContext.CancellationToken);
        await _tracker.RecordAsync(linkId, TestContext.CurrentContext.CancellationToken);
        var snapshot = await _tracker.DrainAsync(TestContext.CurrentContext.CancellationToken);

        Assert.That(snapshot, Has.Count.EqualTo(1));
        Assert.That(snapshot.First().VisitCount, Is.EqualTo(2));
    }

    [Test]
    public async Task DrainAsync_ShouldClearCounts()
    {
        var linkId = new LinkId(Guid.NewGuid());
        await _tracker.RecordAsync(linkId, TestContext.CurrentContext.CancellationToken);

        var firstDrain = await _tracker.DrainAsync(TestContext.CurrentContext.CancellationToken);
        var secondDrain = await _tracker.DrainAsync(TestContext.CurrentContext.CancellationToken);

        Assert.Multiple(() =>
        {
            Assert.That(firstDrain, Has.Count.EqualTo(1));
            Assert.That(secondDrain, Is.Empty);
        });
    }
}
