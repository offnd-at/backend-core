using Application.Formats.Queries.GetSupportedFormats;
using Domain.Enumerations;


namespace OffndAt.Application.UnitTests.Formats.Queries.GetSupportedFormats;internal sealed class GetSupportedFormatsQueryHandlerTests
{
    private GetSupportedFormatsQueryHandler _handler = null!;

    [SetUp]
    public void Setup() => _handler = new GetSupportedFormatsQueryHandler();

    [Test]
    public async Task Handle_ShouldReturnFormatDtoForEachEntryInFormatEnumeration()
    {
        var expected = Format.List.ToList();

        var actual = await _handler.Handle(new GetSupportedFormatsQuery(), CancellationToken.None);

        Assert.Multiple(
            () =>
            {
                Assert.That(actual.HasValue, Is.True);
                Assert.That(expected, Has.Count.EqualTo(actual.Value.Formats.Count()));
            });

        foreach (var format in expected)
        {
            Assert.That(actual.Value.Formats.Any(dto => dto.Value == format.Value), Is.True);
        }
    }
}
