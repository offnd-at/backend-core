namespace OffndAt.Infrastructure.Authentication.Settings;

/// <summary>
///     Represents the authentication settings.
/// </summary>
public sealed class AuthSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "AuthSettings";

    /// <summary>
    ///     Gets or sets the API key.
    /// </summary>
    public required string ApiKey { get; init; }
}
