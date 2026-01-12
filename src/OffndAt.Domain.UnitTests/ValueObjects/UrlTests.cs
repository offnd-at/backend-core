using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Domain.UnitTests.ValueObjects;

internal sealed class UrlTests
{
    [Test]
    public void Create_ShouldReturnError_WhenUrlIsNull()
    {
        var actual = Url.Create(null!);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Url.NullOrEmpty));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenUrlIsEmpty()
    {
        var actual = Url.Create(string.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Url.NullOrEmpty));
        });
    }

    [Test]
    public void Create_ShouldReturnError_WhenUrlIsTooLong()
    {
        var actual = Url.Create(new string('a', Url.MaxLength + 1));

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo(DomainErrors.Url.LongerThanAllowed));
        });
    }
}
