namespace OffndAt.Application;

using Core.Behaviours;
using Core.Constants;
using Domain.Core.Errors;
using Domain.Core.Primitives;
using Domain.ValueObjects;
using FluentValidation;
using FluentValidation.AspNetCore;
using Links.Commands.GenerateLink;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

public static class DependencyInjectionExtensions
{
    /// <summary>
    ///     Registers the application services with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services
            .AddMediator()
            .AddValidatorsFrom()
            .AddFluentValidationAutoValidation()
            .AddResiliencePolicies();

    /// <summary>
    ///     Registers the MediatR and its behaviours with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        return services;
    }

    /// <summary>
    ///     Registers the fluent validators with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddValidatorsFrom(this IServiceCollection services) =>
        services.AddValidatorsFromAssemblyContaining<GenerateLinkCommandValidator>();

    /// <summary>
    ///     Registers the resilience policies with the DI framework.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    private static IServiceCollection AddResiliencePolicies(this IServiceCollection services) =>
        services.AddResiliencePipeline<string, Result<Phrase>>(
            ResiliencePolicies.PhraseGeneratorRetryPolicyName,
            builder => builder.AddRetry(
                new RetryStrategyOptions<Result<Phrase>>
                {
                    Delay = TimeSpan.Zero,
                    BackoffType = DelayBackoffType.Constant,
                    MaxRetryAttempts = 10,
                    ShouldHandle = new PredicateBuilder<Result<Phrase>>().HandleResult(
                        result => result.IsFailure && result.Error == DomainErrors.Phrase.AlreadyInUse)
                }));
}
