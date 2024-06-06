namespace OffndAt.Application.Core.Abstractions.Messaging;

/// <summary>
///     Represents the integration event consumer interface.
/// </summary>
public interface IIntegrationEventConsumer
{
    /// <summary>
    ///     Consumes the specified integration event.
    /// </summary>
    /// <param name="integrationEvent">The integration event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The completed task.</returns>
    Task ConsumeAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
