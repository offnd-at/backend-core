namespace OffndAt.Infrastructure.Core.Settings;

/// <summary>
///     Represents the application's core settings.
/// </summary>
public sealed class ApplicationSettings
{
    public const string SettingsKey = "ApplicationSettings";

    /// <summary>
    ///     Gets or sets the application name.
    /// </summary>
    public string? ApplicationName { get; init; }

    /// <summary>
    ///     Gets or sets the environment name.
    /// </summary>
    public string? Environment { get; init; }

    /// <summary>
    ///     Gets or sets the base domain used for links generation.
    /// </summary>
    public string? BaseDomain { get; init; }

    /// <summary>
    ///     Gets or sets the use https flag, which determines the protocol version for links generation.
    /// </summary>
    public bool UseHttps { get; init; }
}
