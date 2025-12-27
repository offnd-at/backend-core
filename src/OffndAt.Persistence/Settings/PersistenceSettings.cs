namespace OffndAt.Persistence.Settings;

/// <summary>
///     Contains configuration settings for data persistence and database connections.
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
