using Domain.Enumerations;
using Domain.ValueObjects;
using Infrastructure.Words;


namespace OffndAt.Infrastructure.UnitTests.Words;internal sealed class CaseConverterTests
{
    private CaseConverter _converter = null!;

    [SetUp]
    public void Setup() => _converter = new CaseConverter();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Convert_ShouldReturnStringInSpecifiedCase(Format format, string expected)
    {
        var adverb = Word.Create("test adverb").Value;
        var adjective = Word.Create("TEST_Adjective").Value;
        var noun = Word.Create("teST-123-nOUn").Value;

        var actual = _converter.Convert(
            format,
            adverb,
            adjective,
            noun);

        Assert.That(actual, Is.EqualTo(expected));
    }

    private static IEnumerable<object> TestCases()
    {
        yield return new object[] { Format.KebabCase, "test-adverb-test-adjective-te-st-123-n-o-un" };
        yield return new object[] { Format.PascalCase, "TestAdverbTESTAdjectiveTeST-123-nOUn" };
    }
}
