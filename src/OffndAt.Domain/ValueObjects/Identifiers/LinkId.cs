using OffndAt.Domain.Core.Primitives;

namespace OffndAt.Domain.ValueObjects.Identifiers;

/// <summary>
///     Encapsulates a strongly-typed link identifier.
/// </summary>
/// <param name="value">The identifier value.</param>
public sealed class LinkId(Guid value) : EntityId(value);
