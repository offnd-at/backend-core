using System.Linq.Expressions;
using OffndAt.Domain.Entities;
using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Persistence.Specifications.Links;

/// <summary>
///     Defines query criteria for finding links by their unique phrase.
/// </summary>
internal sealed class LinkWithPhraseSpecification : Specification<Link, LinkId>
{
    private readonly Phrase _phrase;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkWithPhraseSpecification" /> class.
    /// </summary>
    /// <param name="phrase">The phrase.</param>
    internal LinkWithPhraseSpecification(Phrase phrase) => _phrase = phrase;

    /// <inheritdoc />
    protected override Expression<Func<Link, bool>> ToExpression() => link => link.Phrase == _phrase;
}
