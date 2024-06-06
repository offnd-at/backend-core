namespace OffndAt.Persistence.Settings;

/// <summary>
///     Represents the persistence settings.
/// </summary>
public sealed class PersistenceSettings
{
    public const string SettingsKey = "PersistenceSettings";

    /// <summary>
    ///     Gets or sets the database connection string.
    /// </summary>
    public string? ConnectionString { get; init; }
}
