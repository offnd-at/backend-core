using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Application;
using OffndAt.Domain;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;
using OffndAt.Persistence.Migrations.Extensions;
using OffndAt.Services.Api;
using OffndAt.Services.Api.Middleware.Extensions;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddApplication()
    .AddDomain()
    .AddApi()
    .AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.UseSwagger()
    .UseSwaggerUI(
        options =>
        {
            foreach (var description in app.DescribeApiVersions())
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
            }
        })
    .UseCors()
    .UseCustomExceptionHandler()
    .EnsureMigrations() // TODO: only viable while running single instance in production
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

app.Run();
