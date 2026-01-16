using OffndAt.Domain.Enumerations;

namespace OffndAt.Domain.ValueObjects;

/// <summary>
///     Represents the contextual information about a link visit.
/// </summary>
/// <param name="Language">The language.</param>
/// <param name="Theme">The theme.</param>
/// <param name="VisitedAt">The date and time when the link was visited, expressed in UTC.</param>
public sealed record LinkVisitedContext(Language Language, Theme Theme, DateTimeOffset VisitedAt);
