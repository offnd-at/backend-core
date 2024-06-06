namespace OffndAt.Infrastructure.Messaging.Settings;

/// <summary>
///     Represents the message broker settings.
/// </summary>
public sealed class MessageBrokerSettings
{
    public const string SettingsKey = "MessageBrokerSettings";

    /// <summary>
    ///     Gets or sets the host name.
    /// </summary>
    public string? Hostname { get; init; }

    /// <summary>
    ///     Gets or sets the port.
    /// </summary>
    public int Port { get; init; }

    /// <summary>
    ///     Gets or sets the user name.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     Gets or sets the password.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    ///     Gets or sets the queue name.
    /// </summary>
    public string? QueueName { get; init; }
}
