namespace OffndAt.Domain.Core.Abstractions;

/// <summary>
///     Marks entities that track creation and modification timestamps.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    ///     Gets the created at date and time in UTC format.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     Gets the modified at date and time in UTC format.
    /// </summary>
    DateTimeOffset? ModifiedAt { get; }
}
