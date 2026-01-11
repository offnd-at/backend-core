using OffndAt.Domain.Core.Exceptions;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.UnitTests.ValueObjects.Identifiers;

internal sealed class LinkIdTests
{
    [Test]
    public void Constructor_ShouldThrowInvariantViolatedException_WhenExecutedWithEmptyGuid() =>
        Assert.Throws<InvariantViolationException>(() =>
        {
            _ = new LinkId(Guid.Empty);
        });
}
