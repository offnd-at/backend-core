using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OffndAt.Application.Abstractions.Links;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.ValueObjects.Identifiers;
using OffndAt.Infrastructure.Links;

namespace OffndAt.Infrastructure.UnitTests.Links;

internal sealed class LinkVisitFlushWorkerTests
{
    private IConfiguration _configuration = null!;
    private ILogger<LinkVisitFlushWorker> _logger = null!;
    private IServiceScope _scope = null!;
    private IServiceScopeFactory _scopeFactory = null!;
    private IServiceProvider _serviceProvider = null!;
    private ILinkVisitSummaryRepository _summaryRepository = null!;
    private ILinkVisitTracker _visitTracker = null!;
    private LinkVisitFlushWorker _worker = null!;

    [SetUp]
    public void Setup()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _scope = Substitute.For<IServiceScope>();
        _visitTracker = Substitute.For<ILinkVisitTracker>();
        _summaryRepository = Substitute.For<ILinkVisitSummaryRepository>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<LinkVisitFlushWorker>>();

        _serviceProvider.GetService(typeof(IServiceScopeFactory)).Returns(_scopeFactory);
        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.GetService(typeof(ILinkVisitTracker)).Returns(_visitTracker);
        _scope.ServiceProvider.GetService(typeof(ILinkVisitSummaryRepository)).Returns(_summaryRepository);

        _worker = new LinkVisitFlushWorker(_serviceProvider, _configuration, _logger);
    }

    [TearDown]
    public void TearDown()
    {
        _scope.Dispose();
        _worker.Dispose();
    }

    [Test]
    public async Task StopAsync_ShouldFlushRemainingData()
    {
        var linkId = new LinkId(Guid.NewGuid());
        var items = new List<LinkTrackerItem>
        {
            new(linkId, 10)
        };
        _visitTracker.DrainAsync(Arg.Any<CancellationToken>()).Returns(items);

        await _worker.StopAsync(TestContext.CurrentContext.CancellationToken);

        await _summaryRepository.Received(1)
            .UpsertTotalVisitsForManyAsync(
                Arg.Is<IReadOnlyCollection<(LinkId, long)>>(list => list.Count == 1 && list.First().Item1 == linkId),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task StopAsync_ShouldNotCallRepository_WhenBatchIsEmpty()
    {
        _visitTracker.DrainAsync(Arg.Any<CancellationToken>()).Returns(new List<LinkTrackerItem>());

        await _worker.StopAsync(TestContext.CurrentContext.CancellationToken);

        await _summaryRepository.DidNotReceiveWithAnyArgs().UpsertTotalVisitsForManyAsync(null!, default);
    }
}
