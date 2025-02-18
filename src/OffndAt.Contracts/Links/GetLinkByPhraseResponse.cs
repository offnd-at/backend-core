namespace OffndAt.Contracts.Links;

using System.ComponentModel;

/// <summary>
///     Represents the get link by phrase response.
/// </summary>
public sealed class GetLinkByPhraseResponse
{
    /// <summary>
    ///     Gets the link.
    /// </summary>
    [Description("The retrieved link.")]
    public required LinkDto Link { get; init; }
}
