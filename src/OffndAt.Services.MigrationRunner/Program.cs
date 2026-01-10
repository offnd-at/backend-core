using Microsoft.AspNetCore.Builder;
using OffndAt.Application;
using OffndAt.Domain;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;
using OffndAt.Services.MigrationRunner.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomain();

builder.Services.AddMediatorWithBehaviours();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddTelemetry(builder.Configuration)
    .AddInfrastructureServices();

builder.Services.AddHostedService<MigrationRunner>();

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.Run();
