using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.ValueObjects.Identifiers;

/// <summary>
///     Represents the link identified value object.
/// </summary>
/// <param name="value">The identifier value.</param>
public sealed class LinkId(Guid value) : EntityId(value);
