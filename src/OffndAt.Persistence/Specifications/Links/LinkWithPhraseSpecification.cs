namespace OffndAt.Persistence.Specifications.Links;

using System.Linq.Expressions;
using Domain.Entities;
using Domain.ValueObjects;
using Domain.ValueObjects.Identifiers;

/// <summary>
///     Represents the specification for determining the link with phrase.
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
