using Microsoft.AspNetCore.Builder;
using OffndAt.Application;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatorWithBehaviours();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddTelemetry(builder.Configuration)
    .AddInfrastructureServices()
    .AddMassTransitConsumer(builder.Configuration);

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.Run();
