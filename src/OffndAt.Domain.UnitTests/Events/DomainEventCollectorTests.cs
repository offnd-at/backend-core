using System.Collections.ObjectModel;
using NSubstitute;
using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Domain.UnitTests.Events;

internal sealed class DomainEventCollectorTests
{
    private DomainEventCollector _collector = null!;

    [SetUp]
    public void Setup() => _collector = new DomainEventCollector();

    [Test]
    public void RaiseEvent_ShouldAddEventToList()
    {
        var domainEvent = Substitute.For<IDomainEvent>();

        _collector.RaiseEvent(domainEvent);

        Assert.That(_collector.GetEvents(), Contains.Item(domainEvent));
    }

    [Test]
    public void GetEvents_ShouldReturnReadonlyList()
    {
        var events = _collector.GetEvents();

        Assert.That(events, Is.TypeOf<ReadOnlyCollection<IDomainEvent>>());
    }

    [Test]
    public void GetEvents_ShouldReturnCollectedEvents()
    {
        var domainEvent = Substitute.For<IDomainEvent>();
        _collector.RaiseEvent(domainEvent);

        var events = _collector.GetEvents();

        Assert.That(events, Has.Count.EqualTo(1));
    }

    [Test]
    public void ClearEvents_ShouldRemoveAllEvents()
    {
        _collector.RaiseEvent(Substitute.For<IDomainEvent>());

        _collector.ClearEvents();

        Assert.That(_collector.GetEvents(), Is.Empty);
    }
}
