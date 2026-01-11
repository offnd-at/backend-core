using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Domain.UnitTests.ValueObjects;

internal sealed class WordTests
{
    [Test]
    public void Create_ShouldReturnError_WhenWordIsNull()
    {
        var actual = Word.Create(null!);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Word.NullOrEmpty));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenWordIsEmpty()
    {
        var actual = Word.Create(string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Word.NullOrEmpty));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenWordIsTooLong()
    {
        var actual = Word.Create(new string('a', Word.MaxLength + 1));

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Word.LongerThanAllowed));
        });
    }
}
