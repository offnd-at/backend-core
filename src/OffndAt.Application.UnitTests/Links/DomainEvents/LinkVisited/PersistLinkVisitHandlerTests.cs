using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using OffndAt.Application.Abstractions.Data;
using OffndAt.Application.Links.DomainEvents.LinkVisited;
using OffndAt.Application.Links.Models;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.Events;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.UnitTests.Links.DomainEvents.LinkVisited;

internal sealed class PersistLinkVisitHandlerTests
{
    private IDbContext _dbContext = null!;
    private DbSet<LinkVisitLogEntry> _dbSet = null!;
    private PersistLinkVisitHandler _handler = null!;
    private IHttpContextAccessor _httpContextAccessor = null!;

    [SetUp]
    public void Setup()
    {
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _dbSet = Substitute.For<DbSet<LinkVisitLogEntry>>();
        _dbContext = Substitute.For<IDbContext>();
        _dbContext.Set<LinkVisitLogEntry>().Returns(_dbSet);

        _handler = new PersistLinkVisitHandler(_dbContext, _httpContextAccessor);
    }

    [Test]
    public async Task Handle_ShouldAddLinkVisitLogEntry_WithCorrectData()
    {
        var linkId = new LinkId(Guid.NewGuid());
        var context = new LinkVisitedContext(Language.English, Theme.None, DateTimeOffset.UtcNow);
        var notification = new LinkVisitedDomainEvent(linkId, context);

        var httpContext = new DefaultHttpContext
        {
            Connection =
            {
                RemoteIpAddress = IPAddress.Parse("127.0.0.1")
            }
        };
        httpContext.Request.Headers.UserAgent = "TestAgent";
        httpContext.Request.Headers.Referer = "https://referrer.com";
        _httpContextAccessor.HttpContext.Returns(httpContext);

        await _handler.Handle(notification, TestContext.CurrentContext.CancellationToken);

        await _dbSet.Received(1)
            .AddAsync(
                Arg.Is<LinkVisitLogEntry>(entry =>
                    entry.LinkId == linkId &&
                    entry.VisitedAt == context.VisitedAt &&
                    entry.IpAddress == "127.0.0.1" &&
                    entry.UserAgent == "TestAgent" &&
                    entry.Referrer == "https://referrer.com"),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ShouldAddLinkVisitLogEntry_WhenHttpContextIsNull()
    {
        var linkId = new LinkId(Guid.NewGuid());
        var context = new LinkVisitedContext(Language.English, Theme.None, DateTimeOffset.UtcNow);
        var notification = new LinkVisitedDomainEvent(linkId, context);

        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        await _handler.Handle(notification, TestContext.CurrentContext.CancellationToken);

        await _dbSet.Received(1)
            .AddAsync(
                Arg.Is<LinkVisitLogEntry>(entry =>
                    entry.IpAddress == null &&
                    entry.UserAgent == null &&
                    entry.Referrer == null),
                Arg.Any<CancellationToken>());
    }
}
