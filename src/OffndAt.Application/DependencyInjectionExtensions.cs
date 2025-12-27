namespace OffndAt.Application;

using System.Reflection;
using Core.Behaviours;
using FluentValidation;
using Links.Commands.GenerateLink;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Contains extensions used to configure DI Container.
/// </summary>
public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers MediatR and its pipeline behaviors in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies containing MediatR services.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMediatorWithBehaviours(this IServiceCollection services, Assembly[]? assemblies = null)
    {
        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies ?? [Assembly.GetCallingAssembly()]));

        _ = services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        _ = services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        _ = services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        return services;
    }

    /// <summary>
    ///     Registers FluentValidation validators in the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<GenerateLinkCommandValidator>();
}
