namespace OffndAt.Domain.UnitTests.Services;

using Domain.Services;
using Enumerations;

internal sealed class VocabularyServiceTests
{
    private VocabularyService _service = new();

    [SetUp]
    public void Setup() => _service = new VocabularyService();

    [Test]
    public void GenerateGrammaticalPropertiesForNounVocabulary_ShouldReturnNoneValues_WhenGivenEnglishLanguage()
    {
        var language = Language.English;

        var (actualNumber, actualGender) = _service.GenerateGrammaticalPropertiesForNounVocabulary(language);

        Assert.Multiple(
            () =>
            {
                Assert.That(actualNumber, Is.EqualTo(GrammaticalNumber.None));
                Assert.That(actualGender, Is.EqualTo(GrammaticalGender.None));
            });
    }

    [Test]
    public void GenerateGrammaticalPropertiesForNounVocabulary_ShouldReturnRealValues_WhenGivenPolishLanguage()
    {
        var language = Language.Polish;

        var (actualNumber, actualGender) = _service.GenerateGrammaticalPropertiesForNounVocabulary(language);

        Assert.Multiple(
            () =>
            {
                Assert.That(actualNumber, Is.Not.EqualTo(GrammaticalNumber.None));
                Assert.That(actualGender, Is.Not.EqualTo(GrammaticalGender.None));
            });
    }
}
