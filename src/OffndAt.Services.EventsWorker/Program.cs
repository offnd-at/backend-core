using Microsoft.AspNetCore.Builder;
using OffndAt.Application;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatorWithBehaviours();

builder.Services.AddPersistenceSettings(builder.Configuration)
    .AddDatabaseContext();

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddTelemetry(builder.Configuration)
    .AddMassTransitConsumer(builder.Configuration, [typeof(Program).Assembly]);

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.Run();
