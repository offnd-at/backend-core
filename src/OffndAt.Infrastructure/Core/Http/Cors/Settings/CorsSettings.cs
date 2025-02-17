namespace OffndAt.Infrastructure.Core.Http.Cors.Settings;

/// <summary>
///     Represents the CORS settings.
/// </summary>
public sealed class CorsSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "CorsSettings";

    /// <summary>
    ///     Gets or sets the origins allowed for CORS policies.
    /// </summary>
    public required string[] AllowedOrigins { get; init; }
}
