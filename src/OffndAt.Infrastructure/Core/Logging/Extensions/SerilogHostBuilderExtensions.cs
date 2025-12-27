using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OffndAt.Infrastructure.Core.Settings;
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
    /// <exception cref="InvalidOperationException">when Environment or ApplicationName is not configured in application settings.</exception>
    public static IHostBuilder UseOffndAtSerilog(this IHostBuilder builder) =>
        builder.UseSerilog((context, services, configuration) =>
        {
            var applicationSettings = services.GetRequiredService<IOptions<ApplicationSettings>>().Value;

            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithClientIp()
                .Enrich.WithCorrelationId()
                .Enrich.WithProperty(nameof(applicationSettings.Environment), applicationSettings.Environment)
                .Enrich.WithProperty(nameof(applicationSettings.ApplicationName), applicationSettings.ApplicationName)
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .ReadFrom.Configuration(context.Configuration);
        });
}
