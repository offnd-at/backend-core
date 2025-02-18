namespace OffndAt.Persistence.Settings;

/// <summary>
///     Represents the persistence settings.
/// </summary>
public sealed class PersistenceSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "PersistenceSettings";

    /// <summary>
    ///     Gets or sets the database connection string.
    /// </summary>
    public required string ConnectionString { get; init; }
}
