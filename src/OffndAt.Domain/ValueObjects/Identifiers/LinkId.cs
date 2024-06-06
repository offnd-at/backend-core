namespace OffndAt.Domain.ValueObjects.Identifiers;

using Core.Primitives;

/// <summary>
///     Represents the link identified value object.
/// </summary>
/// <param name="value">The identifier value.</param>
public sealed class LinkId(Guid value) : EntityId(value);
