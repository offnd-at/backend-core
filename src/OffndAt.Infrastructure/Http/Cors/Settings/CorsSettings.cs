namespace OffndAt.Infrastructure.Http.Cors.Settings;

/// <summary>
///     Represents the CORS settings.
/// </summary>
public sealed class CorsSettings
{
    public const string SettingsKey = "CorsSettings";

    /// <summary>
    ///     Gets or sets the origins allowed for CORS policies.
    /// </summary>
    public string[] AllowedOrigins { get; init; } = [];
}
