using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Application;
using OffndAt.Application.Core.Abstractions.Messaging;
using OffndAt.Domain;
using OffndAt.Domain.Core.Events;
using OffndAt.Infrastructure;
using OffndAt.Infrastructure.Core.Logging.Extensions;
using OffndAt.Persistence;
using OffndAt.Persistence.Migrations.Extensions;
using OffndAt.Services.Api;
using OffndAt.Services.Api.Endpoints.Extensions;
using OffndAt.Services.Api.Middleware.Extensions;
using Scalar.AspNetCore;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddDomain();

builder.Services.AddMediatorWithBehaviours([typeof(ICommand).Assembly, typeof(IDomainEvent).Assembly])
    .AddValidators()
    .AddFluentValidationAutoValidation();

builder.Services.AddPersistence(builder.Configuration)
    .AddInMemoryCache(builder.Configuration);

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddInfrastructureServices()
    .AddCorsPolicies(builder.Configuration)
    .AddResiliencePolicies()
    .AddMassTransitProducer(builder.Configuration)
    .AddAuthorization()
    .AddAuthentication();

builder.Services
    .AddVersioning()
    .AddOpenApiWithExamples()
    .AddApi()
    .AddHttpLogging(options =>
    {
        options.LoggingFields = HttpLoggingFields.RequestBody;
        options.MediaTypeOptions.AddText("application/json");
    });

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.MapEndpointsForAllVersions()
    .UseCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseCustomExceptionHandler()
    .EnsureMigrations() // TODO: only viable while running single instance in production
    .UseHttpsRedirection()
    .UseHttpLogging();

app.MapOpenApi();
app.MapScalarApiReference(
    "docs",
    options => options
        .WithTitle("offnd.at | API reference")
        .SortTagsAlphabetically()
        .SortOperationsByMethod()
        .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Http11)
        .HideClientButton());

app.Run();

namespace OffndAt.Services.Api
{
    /// <summary>
    ///     This is a marker for the application entrypoint class.
    /// </summary>
    public class Program;
}
