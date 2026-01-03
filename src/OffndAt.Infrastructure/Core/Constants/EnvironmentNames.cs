using Microsoft.Extensions.Hosting;

namespace OffndAt.Infrastructure.Core.Constants;

/// <summary>
///     Provides constant environment names used throughout the application.
/// </summary>
public static class EnvironmentNames
{
    /// <summary>
    ///     Represents the environment name for the development stage of the application lifecycle.
    /// </summary>
    public static readonly string Development = Environments.Development;

    /// <summary>
    ///     Represents the environment name for the staging stage of the application lifecycle.
    /// </summary>
    public static readonly string Staging = Environments.Staging;

    /// <summary>
    ///     Represents the environment name for the production stage of the application lifecycle.
    /// </summary>
    public static readonly string Production = Environments.Production;

    /// <summary>
    ///     Specifies the environment name for the testing stage of the application lifecycle.
    /// </summary>
    public static readonly string Testing = nameof(Testing);
}
