using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Application;
using OffndAt.Application.Abstractions.Messaging;
using OffndAt.Application.Links.IntegrationEvents.LinkVisited;
using OffndAt.Domain;
using OffndAt.Domain.Abstractions.Events;
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
    .AddFluentValidationAutoValidation()
    .AddApplicationServices();

builder.Services.AddPersistenceSettings(builder.Configuration)
    .AddDatabaseContext()
    .AddPersistenceServices();

builder.Services.AddInfrastructureSettings(builder.Configuration)
    .AddInMemoryCache(builder.Configuration)
    .AddForwardedHeadersSettings()
    .AddRateLimiting()
    .AddTelemetry(builder.Configuration)
    .AddInfrastructureServices(builder.Environment)
    .AddCorsPolicies(builder.Configuration)
    .AddApiAuthentication(builder.Configuration)
    .AddResiliencePolicies()
    .AddMassTransitForProducerAndConsumer(builder.Configuration, [typeof(LinkVisitedIntegrationEvent).Assembly])
    .AddHealthChecksForDatabaseAndMessaging()
    .AddHealthCheckForGitHubApi();

builder.Services
    .AddVersioning()
    .AddOpenApiWithExamples()
    .AddJsonResponseCompression()
    .AddApi();

builder.Host.UseOffndAtSerilog();

var app = builder.Build();

app.UseForwardedHeaders();

app.MapEndpointsForAllVersions()
    .UseResponseCompression()
    .UseRateLimiter()
    .UseCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseCustomExceptionHandler()
    .EnsureMigrationsIfDevelopment()
    .UseHttpsRedirection();

if (builder.Configuration.GetValue("EnableHttpLogging", false))
{
    app.UseWhen(
        context =>
            !context.Request.Path.StartsWithSegments("/favicon") &&
            !context.Request.Path.StartsWithSegments("/openapi") &&
            !context.Request.Path.StartsWithSegments("/docs") &&
            !context.Request.Path.StartsWithSegments("/health") &&
            !context.Request.Path.StartsWithSegments("/metrics"),
        appBuilder => appBuilder.UseHttpLogging());
}

app.MapHealthChecks(
    "/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

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
