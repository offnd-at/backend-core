using Bogus;
using OffndAt.Contracts.Languages;


namespace OffndAt.Services.Api.Endpoints.Examples.Fakers;/// <summary>
///     Contains fake data generators for languages.
/// </summary>
internal sealed class LanguageFakers
{
    public static Faker<LanguageDto> Language =>
        new Faker<LanguageDto>()
            .RuleFor(f => f.Value, f => f.PickRandom(Domain.Enumerations.Language.List.Select(elem => elem.Value)))
            .RuleFor(f => f.Code, f => f.PickRandom(Domain.Enumerations.Language.List.Select(elem => elem.Code)))
            .RuleFor(f => f.Name, f => f.PickRandom(Domain.Enumerations.Language.List.Select(elem => elem.Name)));
}
