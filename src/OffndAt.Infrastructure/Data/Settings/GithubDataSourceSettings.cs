namespace OffndAt.Infrastructure.Data.Settings;

/// <summary>
///     Represents the CORS settings.
/// </summary>
public sealed class GithubDataSourceSettings
{
    public const string SettingsKey = "GithubDataSourceSettings";

    /// <summary>
    ///     Gets or sets the repository owner.
    /// </summary>
    public string? RepositoryOwner { get; init; }

    /// <summary>
    ///     Gets or sets the repository name.
    /// </summary>
    public string? RepositoryName { get; init; }
}
