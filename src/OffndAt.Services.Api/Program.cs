using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Application;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Logging.Extensions;
using OffndAt.Persistence;
using OffndAt.Persistence.Migrations.Extensions;
using OffndAt.Services.Api;
using OffndAt.Services.Api.Middleware.Extensions;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddInfrastructure(builder.Configuration, "offnd.at API", "v1")
    .AddPersistence(builder.Configuration)
    .AddApplication()
    .AddApi()
    .AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.UseSwagger()
    .UseSwaggerUI()
    .UseCors()
    .UseCustomExceptionHandler()
    .EnsureMigrations() // TODO: only viable while running single instance in production
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

app.Run();
