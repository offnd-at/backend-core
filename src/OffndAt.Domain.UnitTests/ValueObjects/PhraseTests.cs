using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Domain.UnitTests.ValueObjects;

internal sealed class PhraseTests
{
    [Test]
    public void Create_ShouldReturnError_WhenPhraseIsNull()
    {
        var actual = Phrase.Create(null!);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.NullOrEmpty));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenPhraseIsEmpty()
    {
        var actual = Phrase.Create(string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.NullOrEmpty));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenPhraseIsTooLong()
    {
        var actual = Phrase.Create(new string('a', Phrase.MaxLength + 1));

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Phrase.LongerThanAllowed));
        });
    }
}
