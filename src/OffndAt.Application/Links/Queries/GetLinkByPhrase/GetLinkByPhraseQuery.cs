namespace OffndAt.Application.Links.Queries.GetLinkByPhrase;

using Contracts.Links;
using Core.Abstractions.Messaging;

/// <summary>
///     Represents the query used for getting a link by phrase.
/// </summary>
public sealed class GetLinkByPhraseQuery(string phrase) : IQuery<GetLinkByPhraseResponse>
{
    /// <summary>
    ///     Gets the phrase.
    /// </summary>
    public string Phrase { get; } = phrase;
}
