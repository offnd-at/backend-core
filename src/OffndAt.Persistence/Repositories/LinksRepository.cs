using OffndAt.Application.Core.Abstractions.Data;
using OffndAt.Domain.Core.Primitives;
using OffndAt.Domain.Entities;
using OffndAt.Domain.Repositories;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;
using OffndAt.Persistence.Specifications.Links;

namespace OffndAt.Persistence.Repositories;

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
