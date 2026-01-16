using OffndAt.Domain.Entities;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Domain.UnitTests.Entities;

internal sealed class LinkTests
{
    [Test]
    public void Create_ShouldRaiseDomainEvent()
    {
        var actual = Link.Create(
            Phrase.Create("test").Value,
            Url.Create("test").Value,
            Language.English,
            Theme.None);

        Assert.That(actual.DomainEvents, Has.Count.EqualTo(1));
        Assert.That(actual.DomainEvents.First(), Is.TypeOf<LinkCreatedDomainEvent>());
    }

    [Test]
    public void RecordVisit_ShouldRaiseDomainEvent()
    {
        var actual = Link.Create(
            Phrase.Create("test").Value,
            Url.Create("test").Value,
            Language.English,
            Theme.None);

        actual.RecordVisit();

        Assert.That(actual.DomainEvents, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(actual.DomainEvents.First(), Is.TypeOf<LinkCreatedDomainEvent>());
            Assert.That(actual.DomainEvents.Skip(1).First(), Is.TypeOf<LinkVisitedDomainEvent>());
        });
    }
}
