namespace OffndAt.Infrastructure;

using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Application.Core.Abstractions.Phrases;
using Application.Core.Abstractions.Urls;
using Application.Core.Abstractions.Words;
using Core.Settings;
using Data;
using Data.Settings;
using Http.Cors.Settings;
using Messaging;
using Messaging.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Octokit;
using Phrases;
using Urls;
using Words;

public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the infrastructure services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="apiTitle">The API title.</param>
    /// <param name="apiVersion">The API version string.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string apiTitle,
        string apiVersion) =>
        services
            .AddSettings(configuration)
            .AddCors()
            .AddSwagger(apiTitle, apiVersion)
            .AddServices();

    /// <summary>
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IIntegrationEventPublisher, IntegrationEventPublisher>();

        services.AddScoped<ICaseConverter, CaseConverter>();
        services.AddScoped<IPhraseGenerator, PhraseGenerator>();
        services.AddScoped<IUrlMaker, UrlMaker>();
        services.AddScoped<IFileLoader, GithubFileLoader>();
        services.AddScoped<IVocabularyLoader, GithubVocabularyLoader>();
        services.AddScoped<IGitHubClient>(
            serviceProvider =>
            {
                var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>().Value;
                return new GitHubClient(new ProductHeaderValue(applicationSettings.ApplicationName));
            });

        return services;
    }

    /// <summary>
    ///     Registers the CORS policy with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetRequiredService<IOptions<CorsSettings>>().Value;

        services.AddCors(
            options => options.AddDefaultPolicy(
                builder => builder
                    .SetIsOriginAllowed(
                        origin => settings.AllowedOrigins.Any(
                            allowedOrigin => allowedOrigin.Equals(new Uri(origin).Host, StringComparison.OrdinalIgnoreCase)))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));

        return services;
    }

    /// <summary>
    ///     Registers the Swagger API docs with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiTitle">The API title.</param>
    /// <param name="apiVersion">The API version string.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddSwagger(this IServiceCollection services, string apiTitle, string apiVersion)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc(
                    apiVersion,
                    new OpenApiInfo
                    {
                        Title = apiTitle,
                        Version = apiVersion
                    });

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = """
                                      JWT Authorization header using the Bearer scheme. \r\n\r\n
                                                              Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n
                                                              Example: 'Bearer 12345abcdef'
                                      """,
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });
            });

        return services;
    }

    /// <summary>
    ///     Registers the infrastructure settings with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationSettings>(configuration.GetSection(ApplicationSettings.SettingsKey));
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SettingsKey));
        services.Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SettingsKey));
        services.Configure<GithubDataSourceSettings>(configuration.GetSection(GithubDataSourceSettings.SettingsKey));

        return services;
    }
}
