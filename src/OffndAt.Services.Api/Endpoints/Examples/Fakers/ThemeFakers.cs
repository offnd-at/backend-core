using Bogus;
using OffndAt.Contracts.Themes;

namespace OffndAt.Services.Api.Endpoints.Examples.Fakers;

/// <summary>
///     Contains fake data generators for themes.
/// </summary>
internal sealed class ThemeFakers
{
    public static Faker<ThemeDto> Theme =>
        new Faker<ThemeDto>()
            .RuleFor(f => f.Value, f => f.PickRandom(Domain.Enumerations.Theme.List.Select(elem => elem.Value)))
            .RuleFor(f => f.Name, f => f.PickRandom(Domain.Enumerations.Theme.List.Select(elem => elem.Name)));
}
