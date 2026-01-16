using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Links.Models;

/// <summary>
///     Represents a tracked link item, providing link identifier and visits counter.
/// </summary>
/// <param name="LinkId">The link identifier.</param>
/// <param name="VisitCount">The visit count.</param>
public sealed record LinkTrackerItem(LinkId LinkId, long VisitCount);
