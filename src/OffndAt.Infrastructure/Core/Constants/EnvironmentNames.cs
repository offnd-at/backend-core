using Microsoft.Extensions.Hosting;

namespace OffndAt.Infrastructure.Core.Constants;

/// <summary>
///     Provides constant environment names used throughout the application.
/// </summary>
public static class EnvironmentNames
{
    public static readonly string Development = Environments.Development;
    public static readonly string Staging = Environments.Staging;
    public static readonly string Production = Environments.Production;
    public static readonly string Testing = nameof(Testing);
}
