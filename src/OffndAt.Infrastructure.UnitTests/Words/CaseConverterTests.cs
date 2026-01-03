using System.Reflection;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Infrastructure.Words;

namespace OffndAt.Infrastructure.UnitTests.Words;

internal sealed class CaseConverterTests
{
    private CaseConverter _converter = null!;

    [SetUp]
    public void Setup() => _converter = new CaseConverter();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert_ShouldReturnStringInSpecifiedCase(
        Format format,
        string adverbValue,
        string adjectiveValue,
        string nounValue,
        string expected)
    {
        var adverb = Word.Create(adverbValue).Value;
        var adjective = Word.Create(adjectiveValue).Value;
        var noun = Word.Create(nounValue).Value;

        var actual = _converter.Convert(
            format,
            adverb,
            adjective,
            noun);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Convert_ShouldThrowArgumentOutOfRangeException_WhenFormatIsUnknown()
    {
        var adverb = Word.Create("adverb").Value;
        var adjective = Word.Create("adjective").Value;
        var noun = Word.Create("noun").Value;
        var unknownFormat = (Format)Activator.CreateInstance(
            typeof(Format),
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            [999, "unknown"],
            null)!;

        Assert.Throws<ArgumentOutOfRangeException>(() => _converter.Convert(
            unknownFormat,
            adverb,
            adjective,
            noun));
    }

    private static IEnumerable<object> TestCases()
    {
        yield return new object[]
        {
            Format.KebabCase, "test adverb", "TEST_Adjective", "teST-123-nOUn", "test-adverb-test-adjective-te-st-123-n-o-un"
        };
        yield return new object[]
        {
            Format.PascalCase, "test adverb", "TEST_Adjective", "teST-123-nOUn", "TestAdverbTESTAdjectiveTeST-123NOUn"
        };
        yield return new object[]
        {
            Format.KebabCase, "fast", "blue", "car", "fast-blue-car"
        };
        yield return new object[]
        {
            Format.PascalCase, "fast", "blue", "car", "FastBlueCar"
        };
        yield return new object[]
        {
            Format.KebabCase, "very fast", "brightly-colored", "sports_car", "very-fast-brightly-colored-sports-car"
        };
        yield return new object[]
        {
            Format.PascalCase, "very fast", "brightly-colored", "sports_car", "VeryFastBrightlyColoredSportsCar"
        };
    }
}
