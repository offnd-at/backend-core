using OffndAt.Application.Abstractions.Data;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Application.Links.Models;

/// <summary>
///     Represents an aggregated summary of link visit statistics.
/// </summary>
public sealed class LinkVisitSummary : INonEntityDataModel
{
    /// <summary>
    ///     Gets or sets the link identifier.
    /// </summary>
    public required LinkId LinkId { get; init; }

    /// <summary>
    ///     Gets or sets the total number of visits for the link.
    /// </summary>
    public required long TotalVisits { get; init; }
}
