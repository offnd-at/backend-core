namespace OffndAt.Infrastructure.Core.Logging.Settings;

/// <summary>
///     Represents the OpenObserve logger settings.
/// </summary>
public sealed class OpenObserveLoggerSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "OpenObserveLoggerSettings";

    /// <summary>
    ///     Gets or sets the base API URL.
    /// </summary>
    public required string ApiUrl { get; init; }

    /// <summary>
    ///     Gets or sets the organization name.
    /// </summary>
    public required string Organization { get; init; }

    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    ///     Gets or sets the API key.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    ///     Gets or sets the logs stream name.
    /// </summary>
    public required string StreamName { get; init; }
}
