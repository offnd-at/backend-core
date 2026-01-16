using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Contracts.Links.Responses;

namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

/// <summary>
///     Represents the query used for getting a link by phrase.
/// </summary>
/// <param name="phrase">The phrase used to identify the link.</param>
public sealed class GetLinkByPhraseQuery(string phrase) : IQuery<GetLinkByPhraseResponse>
{
    /// <summary>
    ///     Gets the phrase.
    /// </summary>
    public string Phrase { get; } = phrase;
}
