namespace OffndAt.Infrastructure.Core.Logging.Settings;

/// <summary>
///     Represents the OpenObserve logger settings.
/// </summary>
public sealed class OpenObserveLoggerSettings
{
    public const string SettingsKey = "OpenObserveLoggerSettings";

    /// <summary>
    ///     Gets or sets the base API URL.
    /// </summary>
    public string? ApiUrl { get; init; }

    /// <summary>
    ///     Gets or sets the organization name.
    /// </summary>
    public string? Organization { get; init; }

    /// <summary>
    ///     Gets or sets the username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     Gets or sets the API key.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    ///     Gets or sets the logs stream name.
    /// </summary>
    public string? StreamName { get; init; }
}
