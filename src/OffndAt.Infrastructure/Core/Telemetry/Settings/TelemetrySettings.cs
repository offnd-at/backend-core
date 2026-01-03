namespace OffndAt.Infrastructure.Core.Telemetry.Settings;

/// <summary>
///     Represents the telemetry settings.
/// </summary>
public sealed class TelemetrySettings
{
    /// <summary>
    ///     Gets the settings key.
    /// </summary>
    public const string SettingsKey = "TelemetrySettings";

    /// <summary>
    ///     Gets or sets a flag indicating whether telemetry is enabled.
    /// </summary>
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Gets or sets the Open Telemetry exporter endpoint.
    /// </summary>
    public required string ExporterEndpoint { get; init; }
}
