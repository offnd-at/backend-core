namespace OffndAt.Domain.ValueObjects.Identifiers;

using Core.Primitives;

/// <summary>
///     Encapsulates a strongly-typed link identifier.
/// </summary>
/// <param name="value">The identifier value.</param>
public sealed class LinkId(Guid value) : EntityId(value);
