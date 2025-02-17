namespace OffndAt.Infrastructure.Core.Data.Settings;

/// <summary>
///     Represents the GitHub data source settings.
/// </summary>
public sealed class GithubDataSourceSettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "GithubDataSourceSettings";

    /// <summary>
    ///     Gets or sets the repository owner.
    /// </summary>
    public required string RepositoryOwner { get; init; }

    /// <summary>
    ///     Gets or sets the repository name.
    /// </summary>
    public required string RepositoryName { get; init; }
}
