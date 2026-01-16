using OffndAt.Domain.Enumerations;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Links.Models;

/// <summary>
///     Represents a cached link.
/// </summary>
/// <param name="LinkId">The link identifier.</param>
/// <param name="TargetUrl">The link target URL.</param>
/// <param name="Language">The link language.</param>
/// <param name="Theme">The link theme.</param>
public sealed record CachedLink(
    LinkId LinkId,
    Url TargetUrl,
    Language Language,
    Theme Theme);
