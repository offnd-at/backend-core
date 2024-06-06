namespace OffndAt.Infrastructure.Logging.Extensions;

using System.Globalization;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

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
    /// <exception cref="InvalidOperationException">when Environment or ApplicationName is not configured in application settings.</exception>
    public static IHostBuilder UseOffndAtSerilog(this IHostBuilder builder) =>
        builder.UseSerilog(
            (context, services, configuration) =>
            {
                var settings = services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

                configuration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty(
                        nameof(settings.Environment),
                        settings.Environment ??
                        throw new InvalidOperationException($"Missing configuration value for {nameof(settings.Environment)}."))
                    .Enrich.WithProperty(
                        nameof(settings.ApplicationName),
                        settings.ApplicationName ??
                        throw new InvalidOperationException($"Missing configuration value for {nameof(settings.ApplicationName)}."))
                    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                    .ReadFrom.Configuration(context.Configuration);
            });

    /// <summary>
    ///     Registers Serilog with configuration specific to offnd.at application.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <param name="configureLogger">The delegate for configuring the <see cref="LoggerConfiguration" /> that will be used to construct a logger.</param>
    /// <returns>The configured host builder.</returns>
    /// <exception cref="InvalidOperationException">when Environment or ApplicationName is not configured in application settings.</exception>
    public static IHostBuilder UseOffndAtSerilog(
        this IHostBuilder builder,
        Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger) =>
        builder.UseSerilog(
            ((context, services, configuration) =>
            {
                var settings = services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

                configuration
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProperty(
                        nameof(settings.Environment),
                        settings.Environment ??
                        throw new InvalidOperationException($"Missing configuration value for {nameof(settings.Environment)}."))
                    .Enrich.WithProperty(
                        nameof(settings.ApplicationName),
                        settings.ApplicationName ??
                        throw new InvalidOperationException($"Missing configuration value for {nameof(settings.ApplicationName)}."))
                    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                    .ReadFrom.Configuration(context.Configuration);
            }) +
            configureLogger);
}
