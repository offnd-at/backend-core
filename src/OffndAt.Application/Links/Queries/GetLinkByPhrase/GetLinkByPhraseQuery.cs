using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Links.Responses;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

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
    // TODO: rework to use a separate command for that
    public bool ShouldIncrementVisits { get; } = shouldIncrementVisits;
}
