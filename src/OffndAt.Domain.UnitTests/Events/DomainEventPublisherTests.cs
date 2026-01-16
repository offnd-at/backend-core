using MediatR;
using NSubstitute;
using OffndAt.Domain.Abstractions.Events;
using OffndAt.Domain.Events;

namespace OffndAt.Domain.UnitTests.Events;

internal sealed class DomainEventPublisherTests
{
    private IMediator _mediator = null!;
    private DomainEventPublisher _publisher = null!;

    [SetUp]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _publisher = new DomainEventPublisher(_mediator);
    }

    [Test]
    public async Task PublishAsync_ShouldCallMediatorPublish()
    {
        var domainEvent = Substitute.For<IDomainEvent>();

        await _publisher.PublishAsync(domainEvent, TestContext.CurrentContext.CancellationToken);

        await _mediator.Received(1).Publish(domainEvent, Arg.Any<CancellationToken>());
    }
}
