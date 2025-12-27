using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Contracts.Links;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

/// <summary>
///     Query for retrieving a link by its unique phrase.
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
