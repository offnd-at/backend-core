namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

using Contracts.Links;
using Core.Abstractions.Messaging;

/// <summary>
///     Represents the query used for getting a link by phrase.
/// </summary>
public sealed class GetLinkByPhraseQuery(string phrase, bool shouldIncrementVisits) : IQuery<GetLinkByPhraseResponse>
{
    /// <summary>
    ///     Gets the phrase.
    /// </summary>
    public string Phrase { get; } = phrase;

    /// <summary>
    ///     Gets the flag indicating whether link's visits should be incremented upon successful read from the database.
    /// </summary>
    public bool ShouldIncrementVisits { get; } = shouldIncrementVisits;
}
