﻿namespace OffndAt.Domain.Repositories;

using Core.Primitives;
using Entities;
using ValueObjects;

/// <summary>
///     Represents the links repository interface.
/// </summary>
public interface ILinksRepository
{
    /// <summary>
    ///     Gets the link with the specified phrase.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the link with the specified phrase.</returns>
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Checks whether a link with the specified phrase exists.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The maybe instance that may contain the link with the specified phrase.</returns>
    Task<bool> HasAnyWithPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Inserts the specified link to the database.
    /// </summary>
    /// <param name="link">The link to be inserted to the database.</param>
    void Insert(Link link);
}
