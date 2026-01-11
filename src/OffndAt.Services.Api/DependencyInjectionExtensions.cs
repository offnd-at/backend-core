using System.Net.Mime;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using OffndAt.Services.Api.Endpoints.Extensions;

namespace OffndAt.Services.Api;

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
            .AddProblemDetails(cfg => cfg.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);

                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.Add("activityId", activity?.Id);
            })
            .AddEndpointsFromAssemblyContaining<Program>()
            .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true)
            .AddHttpLogging(options =>
            {
                options.LoggingFields = HttpLoggingFields.RequestHeaders |
                    HttpLoggingFields.RequestBody |
                    HttpLoggingFields.ResponseHeaders |
                    HttpLoggingFields.ResponseBody;
                options.MediaTypeOptions.AddText(MediaTypeNames.Application.Json);
                options.MediaTypeOptions.AddText(MediaTypeNames.Application.ProblemJson);
            });

    /// <summary>
    ///     Registers the API versioning services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention()))
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    /// <summary>
    ///     Registers the OpenAPI services, request examples, and response examples with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddOpenApiWithExamples(this IServiceCollection services)
    {
        services.AddOpenApiExamplesFromAssemblyContaining<Program>()
            .AddOpenApi(
                "v1",
                options => options
                    .UseDocumentTitleAndVersion()
                    .UseApiKeyAuthentication()
                    .UseRequestAndResponseExamples());

        return services;
    }
}
