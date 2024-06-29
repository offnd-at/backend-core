namespace OffndAt.Infrastructure;

using Application.Core.Abstractions.Data;
using Application.Core.Abstractions.Messaging;
using Application.Core.Abstractions.Phrases;
using Application.Core.Abstractions.Urls;
using Application.Core.Abstractions.Words;
using Asp.Versioning;
using Asp.Versioning.Conventions;
using Core.Data;
using Core.Data.Settings;
using Core.Http.Cors.Settings;
using Core.Logging.Settings;
using Core.Messaging;
using Core.Messaging.Settings;
using Core.OpenApi;
using Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddSettings(configuration)
            .AddCors()
            .AddSwagger()
            .AddApiVersioning()
            .AddServices();

    /// <summary>
    ///     Registers the service instances with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IIntegrationEventPublisher, IntegrationEventPublisher>();

        services.AddScoped<IIntegrationEventConsumer, IntegrationEventConsumer>();

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
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();
        services.ConfigureOptions<ConfigureVersionedSwaggerGenOptions>();

        return services;
    }

    /// <summary>
    ///     Registers the API versioning services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = false;
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
        services.Configure<OpenObserveLoggerSettings>(configuration.GetSection(OpenObserveLoggerSettings.SettingsKey));

        return services;
    }
}
