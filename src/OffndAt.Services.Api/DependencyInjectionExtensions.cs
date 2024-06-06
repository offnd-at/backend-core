namespace OffndAt.Services.Api;

using Microsoft.AspNetCore.Mvc;

public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the API services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApi(this IServiceCollection services) =>
        services
            .AddRouting(opt => opt.LowercaseUrls = true)
            .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
}
