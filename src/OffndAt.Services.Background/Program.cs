using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Logging.Extensions;
using OffndAt.Services.Background;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddInfrastructure(builder.Configuration, string.Empty, string.Empty)
    .AddBackground();

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.Run();
