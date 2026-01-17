# Dependency Injection Pattern

## Overview

Dependency Injection (DI) is a design pattern that implements Inversion of Control (IoC) for resolving dependencies. Instead of objects creating their dependencies, dependencies are "injected" from the outside, typically via constructor parameters.

## Core Principles

### Inversion of Control

Control of dependency creation is inverted from the consumer to the container:

```csharp
// WITHOUT DI: Class creates its own dependencies
public class GenerateLinkCommandHandler
{
    private readonly ILinkRepository _repository = new LinkRepository();  // ❌ Tight coupling
}

// WITH DI: Dependencies injected
public class GenerateLinkCommandHandler(ILinkRepository repository)  // ✅ Loose coupling
{
    private readonly ILinkRepository _repository = repository;
}
```

### Dependency Inversion Principle

Depend on abstractions, not concretions:

```csharp
// Interface (abstraction) in Domain layer
namespace OffndAt.Domain.Repositories;
public interface ILinkRepository { }

// Implementation (concretion) in Persistence layer
namespace OffndAt.Persistence.Repositories;
internal sealed class LinkRepository : ILinkRepository { }

// Consumer depends on abstraction
public class Handler(ILinkRepository repository) { }
```

## Implementation

### Constructor Injection

**Primary constructors** (C# 12+):

```csharp
internal sealed class GenerateLinkCommandHandler(
    ILinkRepository linkRepository,
    IPhraseGenerator phraseGenerator,
    IUrlMaker urlMaker,
    ResiliencePipelineProvider<string> resiliencePipelineProvider,
    ILogger<GenerateLinkCommandHandler> logger)
    : ICommandHandler<GenerateLinkCommand, GenerateLinkResponse>
{
    // Dependencies automatically available as fields
    public async Task<Result<GenerateLinkResponse>> Handle(
        GenerateLinkCommand request,
        CancellationToken cancellationToken)
    {
        var phraseResult = await phraseGenerator.GenerateAsync(/* ... */);
        // ...
    }
}
```

**Traditional constructors**:

```csharp
internal sealed class GenerateLinkCommandHandler : ICommandHandler<GenerateLinkCommand, GenerateLinkResponse>
{
    private readonly ILinkRepository _linkRepository;
    private readonly IPhraseGenerator _phraseGenerator;
    
    public GenerateLinkCommandHandler(
        ILinkRepository linkRepository,
        IPhraseGenerator phraseGenerator)
    {
        _linkRepository = linkRepository;
        _phraseGenerator = phraseGenerator;
    }
}
```

### Service Registration

Each layer has a `DependencyInjectionExtensions.cs` file:

```csharp
namespace OffndAt.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddScoped<IPhraseGenerator, PhraseGenerator>();
        services.AddScoped<IUrlMaker, UrlMaker>();
        
        return services;
    }
}
```

### Program.cs Registration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register services from each layer
builder.Services
    .AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

var app = builder.Build();
app.Run();
```

## Service Lifetimes

### Transient

New instance every time:

```csharp
services.AddTransient<IPhraseGenerator, PhraseGenerator>();

// Each request gets a new instance
var generator1 = serviceProvider.GetService<IPhraseGenerator>();
var generator2 = serviceProvider.GetService<IPhraseGenerator>();
// generator1 != generator2
```

**Use for**: Lightweight, stateless services

### Scoped

One instance per HTTP request:

```csharp
services.AddScoped<ILinkRepository, LinkRepository>();

// Same instance within a request
var repo1 = serviceProvider.GetService<ILinkRepository>();
var repo2 = serviceProvider.GetService<ILinkRepository>();
// repo1 == repo2 (within same request)
```

**Use for**: Repositories, DbContext, request-specific services

### Singleton

One instance for application lifetime:

```csharp
services.AddSingleton<IMetricsService, MetricsService>();

// Same instance always
var metrics1 = serviceProvider.GetService<IMetricsService>();
var metrics2 = serviceProvider.GetService<IMetricsService>();
// metrics1 == metrics2
```

**Use for**: Caches, configuration, thread-safe services

## Registration Patterns

### Interface to Implementation

```csharp
services.AddScoped<ILinkRepository, LinkRepository>();
```

### Multiple Implementations

```csharp
services.AddScoped<INotificationService, EmailNotificationService>();
services.AddScoped<INotificationService, SmsNotificationService>();

// Inject IEnumerable<INotificationService> to get all
public class NotificationHandler(IEnumerable<INotificationService> notificationServices)
{
    public async Task NotifyAsync()
    {
        foreach (var service in notificationServices)
        {
            await service.SendAsync();
        }
    }
}
```

### Conditional Registration

```csharp
if (builder.Environment.IsDevelopment())
{
    services.AddScoped<IEmailService, FakeEmailService>();
}
else
{
    services.AddScoped<IEmailService, SendGridEmailService>();
}
```

### Factory Pattern

```csharp
services.AddScoped<ILinkRepository>(sp =>
{
    var context = sp.GetRequiredService<OffndAtDbContext>();
    var logger = sp.GetRequiredService<ILogger<LinkRepository>>();
    return new LinkRepository(context, logger);
});
```

## Advanced Patterns

### Keyed Services (.NET 8+)

```csharp
services.AddKeyedScoped<ICache, MemoryCache>("memory");
services.AddKeyedScoped<ICache, RedisCache>("redis");

// Inject specific implementation
public class CacheService([FromKeyedServices("redis")] ICache cache)
{
    // Uses RedisCache
}
```

### Decorator Pattern

```csharp
services.AddScoped<ILinkRepository, LinkRepository>();
services.Decorate<ILinkRepository, CachedLinkRepository>();

// CachedLinkRepository wraps LinkRepository
public class CachedLinkRepository(ILinkRepository inner, IMemoryCache cache) : ILinkRepository
{
    public async Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase)
    {
        if (cache.TryGetValue(phrase, out Link? cachedLink))
            return Maybe<Link>.From(cachedLink!);
        
        var link = await inner.GetByPhraseAsync(phrase);
        if (link.HasValue)
            cache.Set(phrase, link.Value);
        
        return link;
    }
}
```

### Options Pattern

```csharp
// Configuration
public sealed class RabbitMqSettings
{
    public string Host { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

// Registration
services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

// Injection
public class MessagePublisher(IOptions<RabbitMqSettings> options)
{
    private readonly RabbitMqSettings _settings = options.Value;
}
```

## Layer-Specific Registration

### Domain Layer

```csharp
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton<IDomainEventCollector, DomainEventCollector>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        
        return services;
    }
}
```

### Application Layer

```csharp
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
```

### Infrastructure Layer

```csharp
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) => { /* ... */ });
        });
        
        services.AddOpenTelemetry()
            .WithMetrics(/* ... */)
            .WithTracing(/* ... */);
        
        services.AddMemoryCache();
        
        return services;
    }
}
```

### Persistence Layer

```csharp
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OffndAtDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));
        
        services.AddScoped<ILinkRepository, LinkRepository>();
        
        return services;
    }
}
```

## Testing

### Unit Tests with Mocking

```csharp
[Test]
public async Task Handle_ValidCommand_CreatesLink()
{
    // Arrange - Create mocks
    var mockRepository = Substitute.For<ILinkRepository>();
    var mockPhraseGenerator = Substitute.For<IPhraseGenerator>();
    
    mockPhraseGenerator.GenerateAsync(Arg.Any<Format>(), Arg.Any<Language>(), Arg.Any<Theme>())
        .Returns(Result.Success(Phrase.Create("test-phrase").Value));
    
    // Inject mocks
    var handler = new GenerateLinkCommandHandler(
        mockRepository,
        mockPhraseGenerator,
        /* ... */);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    mockRepository.Received(1).Insert(Arg.Any<Link>());
}
```

### Integration Tests

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace services for testing
            services.RemoveAll<ILinkRepository>();
            services.AddScoped<ILinkRepository, InMemoryLinkRepository>();
        });
    }
}

[Test]
public async Task GenerateLink_ValidRequest_ReturnsCreated()
{
    // Arrange
    await using var factory = new CustomWebApplicationFactory();
    var client = factory.CreateClient();
    
    // Act
    var response = await client.PostAsJsonAsync("/api/v1/links", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

## Benefits

### ✅ Testability

Easy to mock dependencies:

```csharp
var mockRepository = Substitute.For<ILinkRepository>();
var handler = new Handler(mockRepository);
```

### ✅ Flexibility

Swap implementations without changing code:

```csharp
// Development
services.AddScoped<IEmailService, FakeEmailService>();

// Production
services.AddScoped<IEmailService, SendGridEmailService>();
```

### ✅ Maintainability

Dependencies are explicit and centralized:

```csharp
// Clear what the class needs
public class Handler(
    IRepository repository,
    ILogger logger,
    ICache cache)
```

### ✅ Loose Coupling

Classes depend on abstractions:

```csharp
// Depends on interface, not implementation
public class Handler(ILinkRepository repository)
```

## Anti-Patterns

### ❌ Service Locator

```csharp
// BAD: Service locator anti-pattern
public class Handler
{
    public void Execute()
    {
        var repository = ServiceLocator.Get<ILinkRepository>();
    }
}

// GOOD: Constructor injection
public class Handler(ILinkRepository repository)
{
    public void Execute()
    {
        // Use repository
    }
}
```

### ❌ Too Many Dependencies

```csharp
// BAD: Too many dependencies (God Object)
public class Handler(
    IDep1 dep1, IDep2 dep2, IDep3 dep3, IDep4 dep4,
    IDep5 dep5, IDep6 dep6, IDep7 dep7, IDep8 dep8)

// GOOD: Refactor into smaller classes
public class Handler(IDep1 dep1, IDep2 dep2)
```

### ❌ Circular Dependencies

```csharp
// BAD: A depends on B, B depends on A
public class ServiceA(ServiceB serviceB) { }
public class ServiceB(ServiceA serviceA) { }

// GOOD: Extract common interface
public class ServiceA(ISharedService service) { }
public class ServiceB(ISharedService service) { }
```

## Related Patterns

- [Clean Architecture](./clean-architecture.md) - DI enables dependency inversion
- [Repository Pattern](./repository-pattern.md) - Repositories injected into handlers
- [CQRS](./cqrs.md) - Handlers receive dependencies via DI

## Further Reading

- [Dependency Injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) by Microsoft
- [Dependency Injection Principles, Practices, and Patterns](https://www.manning.com/books/dependency-injection-principles-practices-patterns) by Steven van Deursen and Mark Seemann
