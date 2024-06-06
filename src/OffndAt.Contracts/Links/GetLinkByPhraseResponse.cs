namespace OffndAt.Contracts.Links;

/// <summary>
///     Represents the get link by phrase response.
/// </summary>
/// <param name="link">The link.</param>
public sealed class GetLinkByPhraseResponse(LinkDto link)
{
    /// <summary>
    ///     Gets the link.
    /// </summary>
    public LinkDto Link { get; } = link;
}
