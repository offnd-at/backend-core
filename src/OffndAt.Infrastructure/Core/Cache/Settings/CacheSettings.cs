namespace OffndAt.Infrastructure.Core.Cache.Settings;

/// <summary>
///     Represents the cache settings.
/// </summary>
public sealed class CacheSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "CacheSettings";

    /// <summary>
    ///     Gets or sets the long time-to-live for cache entries.
    /// </summary>
    public required TimeSpan LongTtl { get; init; } = TimeSpan.FromHours(1);

    /// <summary>
    ///     Gets or sets the short time-to-live for cache entries.
    /// </summary>
    public required TimeSpan ShortTtl { get; init; } = TimeSpan.FromMinutes(1);
}
