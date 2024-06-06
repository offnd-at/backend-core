namespace OffndAt.Persistence.Repositories;

using Application.Core.Abstractions.Data;
using Domain.Core.Primitives;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;
using Specifications.Links;

/// <summary>
///     Represents the links repository.
/// </summary>
/// <param name="dbContext">The database context.</param>
internal sealed class LinksRepository(IDbContext dbContext) : GenericRepository<Link, LinkId>(dbContext), ILinksRepository
{
    /// <inheritdoc />
    public Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default) =>
        FirstOrDefaultAsync(new LinkWithPhraseSpecification(phrase), cancellationToken);

    /// <inheritdoc />
    public Task<bool> HasAnyWithPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default) =>
        AnyAsync(new LinkWithPhraseSpecification(phrase), cancellationToken);
}
