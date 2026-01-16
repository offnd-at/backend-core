using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.ValueObjects.Identifiers;

/// <summary>
///     Represents the link identified value object.
/// </summary>
public sealed record LinkId : EntityId
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkId" /> record.
    /// </summary>
    /// <param name="value">The identifier value.</param>
    public LinkId(Guid value)
        : base(value)
    {
    }
}
