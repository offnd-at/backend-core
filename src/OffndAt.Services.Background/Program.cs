using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Services.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddInfrastructure(builder.Configuration)
    .AddBackground();

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.Run();
