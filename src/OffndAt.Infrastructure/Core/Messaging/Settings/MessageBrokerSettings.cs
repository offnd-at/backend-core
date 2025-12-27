namespace OffndAt.Infrastructure.Core.Messaging.Settings;

/// <summary>
///     Contains configuration settings for RabbitMQ message broker connections.
/// </summary>
public sealed class MessageBrokerSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "MessageBrokerSettings";

    /// <summary>
    ///     Gets or sets the host name.
    /// </summary>
    public required string Hostname { get; init; }

    /// <summary>
    ///     Gets or sets the port.
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    ///     Gets or sets the user name.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///     Gets or sets the password.
    /// </summary>
    public required string Password { get; init; }
}
