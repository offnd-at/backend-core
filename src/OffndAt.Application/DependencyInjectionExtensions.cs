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
    ///     Registers the MediatR with the DI framework without any additional configurations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddCleanMediator(this IServiceCollection services)
    {
        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetCallingAssembly()));

        return services;
    }

    /// <summary>
    ///     Registers the MediatR and its behaviours with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddMediatorWithBehaviours(this IServiceCollection services)
    {
        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        _ = services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        _ = services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        _ = services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        return services;
    }

    /// <summary>
    ///     Registers the fluent validators with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<GenerateLinkCommandValidator>();
}
