namespace OffndAt.Services.Api.Endpoints.Examples.Fakers;

using Bogus;
using OffndAt.Contracts.Formats;

/// <summary>
///     Contains fake data generators for formats.
/// </summary>
internal sealed class FormatFakers
{
    public static Faker<FormatDto> Format =>
        new Faker<FormatDto>()
            .RuleFor(f => f.Value, f => f.PickRandom(Domain.Enumerations.Format.List.Select(elem => elem.Value)))
            .RuleFor(f => f.Name, f => f.PickRandom(Domain.Enumerations.Format.List.Select(elem => elem.Name)));
}
