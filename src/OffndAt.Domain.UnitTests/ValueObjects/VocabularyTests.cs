using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Domain.UnitTests.ValueObjects;

internal sealed class VocabularyTests
{
    private static readonly VocabularyDescriptor Descriptor = new(
        Language.English,
        Theme.None,
        Offensiveness.Offensive,
        GrammaticalNumber.None,
        GrammaticalGender.None,
        PartOfSpeech.Noun);

    [Test]
    public void Create_ShouldReturnError_WhenListOfWordsIsNull()
    {
        var actual = Vocabulary.Create(Descriptor, null!);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.EmptyWordsList));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenListOfWordsIsEmpty()
    {
        var actual = Vocabulary.Create(Descriptor, []);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Vocabulary.EmptyWordsList));
        });
    }
}
