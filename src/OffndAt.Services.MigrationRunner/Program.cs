using System.Diagnostics;
using OffndAt.Application;
using OffndAt.Domain;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;
using OffndAt.Services.MigrationRunner;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDomain();

builder.Services.AddMediatorWithBehaviours();

builder.Services.AddPersistenceSettings(builder.Configuration)
    .AddDatabaseContext();

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddTelemetry(builder.Configuration);

builder.Services.AddOffndAtSerilog(builder.Configuration);

var host = builder.Build();

return await RunMigrationsAsync(host.Services);

static async Task<int> RunMigrationsAsync(IServiceProvider services)
{
    const int maxRetries = 10;
    var initialRetryDelay = TimeSpan.FromSeconds(2);

    var logger = services.GetRequiredService<ILogger<Program>>();
    var stopwatch = Stopwatch.StartNew();

    logger.LogInformation("Migration runner starting...");

    try
    {
        await MigrationOrchestrator.RunMigrationsWithRetryAsync(
            services,
            logger,
            maxRetries,
            initialRetryDelay);

        stopwatch.Stop();
        logger.LogInformation(
            "Migration runner completed successfully in {ElapsedMilliseconds}ms",
            stopwatch.ElapsedMilliseconds);

        return 0;
    }
    catch (OperationCanceledException)
    {
        logger.LogWarning("Migration runner was cancelled");

        return 1;
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        logger.LogError(ex, "Migration runner failed after {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);

        return 1;
    }
}
