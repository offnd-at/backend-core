using Bogus;
using OffndAt.Contracts.Links.Dtos;

namespace OffndAt.Services.Api.Endpoints.Examples.Fakers;

/// <summary>
///     Contains fake data generators for links.
/// </summary>
internal sealed class LinkFakers
{
    public static Faker<LinkDto> Link =>
        new Faker<LinkDto>()
            .RuleFor(f => f.TargetUrl, f => f.Internet.Url())
            .RuleFor(f => f.Visits, f => f.Random.Int(0, 1000000));
}
