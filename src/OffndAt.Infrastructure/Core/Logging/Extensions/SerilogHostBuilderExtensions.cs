using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OffndAt.Infrastructure.Core.Settings;
using OffndAt.Infrastructure.Core.Telemetry.Settings;
using Serilog;

namespace OffndAt.Infrastructure.Core.Logging.Extensions;

/// <summary>
///     Contains extensions used to configure Serilog.
/// </summary>
public static class SerilogHostBuilderExtensions
{
    /// <summary>
    ///     Registers Serilog with configuration specific to offnd.at application.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <returns>The configured host builder.</returns>
    /// <exception cref="InvalidOperationException">when Environment or AppName is not configured in application settings.</exception>
    public static IHostBuilder UseOffndAtSerilog(this IHostBuilder builder) =>
        builder.UseSerilog((context, services, configuration) =>
        {
            var telemetrySettings = services.GetRequiredService<IOptions<TelemetrySettings>>().Value;
            var applicationSettings = services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithCorrelationId(addValueIfHeaderAbsence: true)
                .Enrich.WithProperty(nameof(applicationSettings.Environment), applicationSettings.Environment)
                .Enrich.WithProperty(nameof(applicationSettings.AppName), applicationSettings.AppName)
                .Enrich.WithProperty(nameof(applicationSettings.Version), applicationSettings.Version)
                .Destructure.ToMaximumDepth(10)
                .Destructure.ToMaximumStringLength(2048)
                .Destructure.ToMaximumCollectionCount(64)
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .WriteTo.OpenTelemetry(options => options.Endpoint = telemetrySettings.ExporterEndpoint)
                .ReadFrom.Configuration(context.Configuration);
        });

    /// <summary>
    ///     Registers Serilog with configuration specific to offnd.at application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddOffndAtSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            var telemetrySettings = serviceProvider.GetRequiredService<IOptions<TelemetrySettings>>().Value;
            var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            loggerConfiguration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithCorrelationId(addValueIfHeaderAbsence: true)
                .Enrich.WithProperty(nameof(applicationSettings.Environment), applicationSettings.Environment)
                .Enrich.WithProperty(nameof(applicationSettings.AppName), applicationSettings.AppName)
                .Enrich.WithProperty(nameof(applicationSettings.Version), applicationSettings.Version)
                .Destructure.ToMaximumDepth(10)
                .Destructure.ToMaximumStringLength(2048)
                .Destructure.ToMaximumCollectionCount(64)
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .WriteTo.OpenTelemetry(options => options.Endpoint = telemetrySettings.ExporterEndpoint)
                .ReadFrom.Configuration(configuration);
        });

        return services;
    }
}
