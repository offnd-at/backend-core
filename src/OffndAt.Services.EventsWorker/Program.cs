using Microsoft.AspNetCore.Builder;
using OffndAt.Application;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;
using OffndAt.Services.EventsWorker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCleanMediator();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddTelemetry(builder.Configuration)
    .AddInfrastructureServices();

builder.Services.AddEventsWorker(builder.Configuration);

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.Run();
