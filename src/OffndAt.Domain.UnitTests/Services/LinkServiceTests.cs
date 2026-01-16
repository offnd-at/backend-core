using NSubstitute;
using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.Services;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.UnitTests.Services;

internal sealed class LinkServiceTests
{
    private IDomainEventCollector _domainEventCollector = null!;
    private LinkService _linkService = null!;

    [SetUp]
    public void Setup()
    {
        _domainEventCollector = Substitute.For<IDomainEventCollector>();
        _linkService = new LinkService(_domainEventCollector);
    }

    [Test]
    public async Task RecordLinkVisitAsync_ShouldRaiseLinkVisitedDomainEvent()
    {
        var linkId = new LinkId(Guid.NewGuid());
        var context = new LinkVisitedContext(Language.English, Theme.None, DateTimeOffset.UtcNow);

        await _linkService.RecordLinkVisitAsync(linkId, context);

        _domainEventCollector.Received(1)
            .RaiseEvent(
                Arg.Is<LinkVisitedDomainEvent>(e =>
                    e.LinkId == linkId &&
                    e.Context == context));
    }
}
