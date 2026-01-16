using Bogus;
using OffndAt.Contracts.Links.Dtos;

namespace OffndAt.Services.Api.Endpoints.Examples.Fakers;

/// <summary>
///     Contains fake data generators for links.
/// </summary>
internal sealed class LinkFakers
{
    public static Faker<LinkVisitDto> LinkVisit =>
        new Faker<LinkVisitDto>()
            .RuleFor(l => l.VisitedAt, f => f.Date.RecentOffset())
            .RuleFor(l => l.IpAddress, f => f.Internet.Ip())
            .RuleFor(l => l.UserAgent, f => f.Internet.UserAgent())
            .RuleFor(l => l.Referrer, f => f.Internet.Url());
}
