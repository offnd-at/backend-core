namespace OffndAt.Application.UnitTests.Themes.Queries.GetSupportedThemes;

using Application.Themes.Queries.GetSupportedThemes;
using Domain.Enumerations;

internal sealed class GetSupportedThemesQueryHandlerTests
{
    private GetSupportedThemesQueryHandler _handler = null!;

    [SetUp]
    public void Setup() => _handler = new GetSupportedThemesQueryHandler();

    [Test]
    public async Task Handle_ShouldReturnThemeDtoForEachEntryInThemeEnumeration()
    {
        var expected = Theme.List.ToList();

        var actual = await _handler.Handle(new GetSupportedThemesQuery(), CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(expected, Has.Count.EqualTo(actual.Value.Themes.Count()));
            });

        foreach (var theme in expected)
        {
            Assert.That(actual.Value.Themes.Any(dto => dto.Value == theme.Value), Is.True);
        }
    }
}
