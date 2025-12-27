namespace OffndAt.Persistence.Specifications;

using System.Linq.Expressions;
using Domain.Core.Primitives;

/// <summary>
///     Provides base functionality for implementing the Specification pattern for query filtering.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TEntityId">The entity identifier type.</typeparam>
public abstract class Specification<TEntity, TEntityId>
    where TEntity : Entity<TEntityId>
    where TEntityId : EntityId
{
    /// <summary>
    ///     Converts the specification to an expression predicate.
    /// </summary>
    /// <returns>The expression predicate.</returns>
    protected abstract Expression<Func<TEntity, bool>> ToExpression();

    /// <summary>
    ///     Checks if the specified entity satisfies this specification.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>True if the entity satisfies the specification, otherwise false.</returns>
    public bool IsSatisfiedBy(TEntity entity) => ToExpression().Compile()(entity);

    /// <summary>
    ///     Implicitly converts a <see cref="Specification{T, K}" /> to an <see cref="Expression{T}" />.
    /// </summary>
    /// <param name="specification">The specification to convert.</param>
    /// <returns>An <see cref="Expression{T}" /> that represents the specification.</returns>
    public static implicit operator Expression<Func<TEntity, bool>>(Specification<TEntity, TEntityId> specification) =>
        specification.ToExpression();
}
