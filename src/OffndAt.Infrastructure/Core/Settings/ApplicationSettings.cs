namespace OffndAt.Infrastructure.Core.Settings;

/// <summary>
///     Represents the application's core settings.
/// </summary>
public sealed class ApplicationSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "ApplicationSettings";

    /// <summary>
    ///     Gets or sets the application name.
    /// </summary>
    public required string AppName { get; init; }

    /// <summary>
    ///     Gets or sets the application version.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    ///     Gets or sets the environment name.
    /// </summary>
    public required string Environment { get; init; }

    /// <summary>
    ///     Gets or sets the base domain used for links generation.
    /// </summary>
    public string? BaseDomain { get; init; }

    /// <summary>
    ///     Gets or sets the use https flag, which determines the protocol version for links generation.
    /// </summary>
    public bool? UseHttps { get; init; }
}
