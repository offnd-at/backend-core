using OffndAt.Domain.Core.Errors;
using OffndAt.Domain.Core.Exceptions;
using OffndAt.Domain.Core.Guards;

namespace OffndAt.Domain.UnitTests.Core.Guards;

internal sealed class GuardTests
{
    private const string ErrorMessage = "test-error";

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenStringValueIsNull()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty((string?)null, ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenStringValueIsEmpty()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty(string.Empty, ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenStringValueIsWhitespace()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty("   ", ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldNotThrow_WhenStringValueIsValid() =>
        Assert.DoesNotThrow(() => Guard.AgainstEmpty("valid", ErrorMessage));

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenGuidValueIsNull()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty((Guid?)null, ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenGuidValueIsEmpty()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty(Guid.Empty, ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldNotThrow_WhenGuidValueIsValid() =>
        Assert.DoesNotThrow(() => Guard.AgainstEmpty(Guid.NewGuid(), ErrorMessage));

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenEnumerableValueIsNull()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty<int>(null, ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldThrow_WhenEnumerableValueIsEmpty()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstEmpty(Array.Empty<int>(), ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstEmpty_ShouldNotThrow_WhenEnumerableValueHasElements() =>
        Assert.DoesNotThrow(() => Guard.AgainstEmpty([1], ErrorMessage));

    [Test]
    public void AgainstNull_ShouldThrow_WhenValueIsNull()
    {
        var exception = Assert.Throws<InvariantViolationException>(() => Guard.AgainstNull<object>(null, ErrorMessage));

        Assert.That(exception.Error, Is.EqualTo(DomainErrors.General.InvariantViolated(ErrorMessage)));
    }

    [Test]
    public void AgainstNull_ShouldNotThrow_WhenValueIsNotNull() => Assert.DoesNotThrow(() => Guard.AgainstNull(new object(), ErrorMessage));
}
