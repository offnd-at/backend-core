using System.ComponentModel;
using OffndAt.Contracts.Links.Dtos;

namespace OffndAt.Contracts.Links.Responses;

/// <summary>
///     Represents the get link by phrase response.
/// </summary>
public sealed class GetLinkByPhraseResponse
{
    /// <summary>
    ///     Gets or sets the identifier.
    /// </summary>
    [Description("The unique identifier of the link.")]
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets or sets the phrase.
    /// </summary>
    [Description("The unique phrase of the link.")]
    public required string Phrase { get; init; }

    /// <summary>
    ///     Gets the target URL.
    /// </summary>
    [Description("The target URL to which the link will redirect.")]
    public required string TargetUrl { get; init; }

    /// <summary>
    ///     Gets or sets the language identifier.
    /// </summary>
    [Description("The identifier of the language used for the link.")]
    public required int LanguageId { get; init; }

    /// <summary>
    ///     Gets or sets the theme identifier.
    /// </summary>
    [Description("The identifier of the theme applied to the link.")]
    public required int ThemeId { get; init; }

    /// <summary>
    ///     Gets the visits count.
    /// </summary>
    [Description("The number of times the link has been visited.")]
    public required long Visits { get; init; }

    /// <summary>
    ///     Gets or sets a collection of details of the most recent visits (up to 10).
    /// </summary>
    [Description("A collection of details of the 10 most recent visits to the link.")]
    public required IEnumerable<LinkVisitDto> RecentVisits { get; init; }

    /// <summary>
    ///     Gets or sets the date and time when the link was created, expressed in UTC.
    /// </summary>
    [Description("The date and time when the link was created, expressed in UTC.")]
    public required DateTimeOffset CreatedAt { get; init; }
}
