﻿namespace OffndAt.Services.Api;

using Asp.Versioning;
using Asp.Versioning.Conventions;
using Endpoints.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the API services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApi(this IServiceCollection services) =>
        services
            .AddProblemDetails(
                cfg => cfg.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);

                    var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.Add("activityId", activity?.Id);
                })
            .AddEndpointsFromAssemblyContaining<Program>()
            .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

    /// <summary>
    ///     Registers the API versioning services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        _ = services.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
            .AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention()))
            .AddApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });

        return services;
    }

    /// <summary>
    ///     Registers the OpenAPI services, request examples and response examples with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddOpenApiWithExamples(this IServiceCollection services)
    {
        _ = services.AddOpenApiExamplesFromAssemblyContaining<Program>()
            .AddOpenApi(
                "v1",
                options => options
                    .UseDocumentTitleAndVersion()
                    .UseJwtBearerAuthentication()
                    .UseProblemDetailsExampleResponses()
                    .UseRequestAndResponseExamples(services.BuildServiceProvider()));

        return services;
    }
}
