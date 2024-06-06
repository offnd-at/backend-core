namespace OffndAt.Application.UnitTests.Languages.Queries.GetSupportedLanguages;

using Application.Languages.Queries.GetSupportedLanguages;
using Domain.Enumerations;

internal sealed class GetSupportedLanguagesQueryHandlerTests
{
    private GetSupportedLanguagesQueryHandler _handler = null!;

    [SetUp]
    public void Setup() => _handler = new GetSupportedLanguagesQueryHandler();

    [Test]
    public async Task Handle_ShouldReturnLanguageDtoForEachEntryInLanguageEnumeration()
    {
        var expected = Language.List.ToList();

        var actual = await _handler.Handle(new GetSupportedLanguagesQuery(), CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(expected, Has.Count.EqualTo(actual.Value.Languages.Count()));
            });

        foreach (var language in expected)
        {
            Assert.That(actual.Value.Languages.Any(dto => dto.Value == language.Value), Is.True);
        }
    }
}
