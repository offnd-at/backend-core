using OffndAt.Application.Links.ReadModels;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.ValueObjects;

namespace OffndAt.Application.Abstractions.Links;

/// <summary>
///     Provides an interface for querying information related to links.
/// </summary>
public interface ILinkQueryService
{
    /// <summary>
    ///     Retrieves a link matching the given phrase.
    /// </summary>
    /// <param name="phrase">The unique phrase for the link.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     A <see cref="Maybe{T}" /> instance containing the <see cref="LinkReadModel" />,
    ///     or <see cref="Maybe{T}.None" /> if no link with the specified phrase exists.
    /// </returns>
    Task<Maybe<LinkReadModel>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
}
