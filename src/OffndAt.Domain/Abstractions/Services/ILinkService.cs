using OffndAt.Domain.ValueObjects;
using OffndAt.Domain.ValueObjects.Identifiers;

namespace OffndAt.Domain.Abstractions.Services;

/// <summary>
///     Represents the link service interface.
/// </summary>
public interface ILinkService
{
    /// <summary>
    /// </summary>
    /// <param name="id">The unique identifier of the link that was visited.</param>
    /// <param name="context">The context of the link visit.</param>
    /// <returns>An awaitable task.</returns>
    Task RecordLinkVisitAsync(LinkId id, LinkVisitedContext context);
}
