using System.ComponentModel;

namespace OffndAt.Contracts.Links;

/// <summary>
///     Response model for the get link by phrase API endpoint.
/// </summary>
public sealed class GetLinkByPhraseResponse
{
    /// <summary>
    ///     Gets the link.
    /// </summary>
    [Description("The retrieved link.")]
    public required LinkDto Link { get; init; }
}
