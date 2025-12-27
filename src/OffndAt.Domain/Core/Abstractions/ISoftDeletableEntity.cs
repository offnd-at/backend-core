namespace OffndAt.Domain.Core.Abstractions;

/// <summary>
///     Marks entities that support soft deletion instead of physical removal.
/// </summary>
public interface ISoftDeletableEntity
{
    /// <summary>
    ///     Gets the date and time in UTC format the entity was deleted on.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    ///     Gets a value indicating whether the entity has been deleted.
    /// </summary>
    bool IsDeleted { get; }
}
