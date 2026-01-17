# Architecture Details

## Domain Layer Deep Dive

### Core Primitives

#### AggregateRoot
The base class for all aggregate roots in the system:

```csharp
public abstract class AggregateRoot<TEntityId> : SoftDeletableEntity<TEntityId>, IAggregateRoot 
    where TEntityId : EntityId
{
    private readonly List<IDomainEvent> _domainEvents = [];
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

**Key Features**:
- Manages domain events collection
- Provides soft-delete capability
- Enforces strongly-typed entity IDs
- Encapsulates domain event lifecycle

#### Entity
Base class for entities with identity:

```csharp
public abstract class Entity<TEntityId> where TEntityId : EntityId
{
    public TEntityId Id { get; protected init; }
    public DateTime CreatedAtUtc { get; protected init; }
    public DateTime? ModifiedAtUtc { get; protected set; }
}
```

**Characteristics**:
- Strongly-typed IDs using `EntityId` record types
- Audit timestamps (created/modified)
- Protected setters for encapsulation

#### Value Objects
Immutable objects defined by their attributes:

```csharp
public sealed record Url
{
    public string Value { get; private init; }
    
    public static Result<Url> Create(string value)
    {
        // Validation logic
        return new Url { Value = value };
    }
}
```

**Examples**:
- `Url`: Validated URL strings
- `Phrase`: Generated short link phrases
- `Email`: Validated email addresses

### Domain Entities

#### Link Aggregate
The primary aggregate root representing a short link:

```csharp
public sealed class Link : AggregateRoot<LinkId>
{
    public Phrase Phrase { get; }
    public Url TargetUrl { get; }
    public Language Language { get; }
    public Theme Theme { get; }
    
    public static Link Create(
        Phrase phrase, 
        Url targetUrl, 
        Language language, 
        Theme theme)
    {
        var link = new Link(phrase, targetUrl, language, theme);
        link.RaiseDomainEvent(new LinkCreatedDomainEvent(link.Id, link.Language, link.Theme));
        return link;
    }
    
    public void RecordVisit()
    {
        RaiseDomainEvent(new LinkVisitedDomainEvent(Id, new LinkVisitedContext(Language, Theme, DateTimeOffset.UtcNow)));
    }
}
```

**Invariants**:
- Phrase must be unique
- Target URL must be valid
- Language and Theme are required

### Domain Events

#### Event Types

**LinkCreatedDomainEvent**:
```csharp
public sealed record LinkCreatedDomainEvent(LinkId LinkId) : IDomainEvent;
```

**LinkVisitedDomainEvent**:
```csharp
public sealed record LinkVisitedDomainEvent(LinkId LinkId) : IDomainEvent;
```

#### Event Flow
1. Aggregate raises domain event
2. Event stored in aggregate's event collection
3. After successful persistence, events published via MediatR
4. Domain event handlers execute within same transaction
5. Integration events published to message bus

### Enumerations

Smart enumerations provide type-safe, extensible alternatives to C# enums:

```csharp
public sealed class Language : Enumeration<Language>
{
    public static readonly Language English = new(1, "en", "English");
    public static readonly Language Polish = new(2, "pl", "Polish");
    
    public string Code { get; }
    public string Name { get; }
    
    private Language(int value, string code, string name) : base(value)
    {
        Code = code;
        Name = name;
    }
}
```

**Available Enumerations**:
- `Language`: Supported languages for phrase generation
- `Theme`: Thematic categories for phrases
- `Format`: Phrase format patterns

### Repository Interfaces

```csharp
public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
    Task<bool> HasAnyWithPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
    void Insert(Link link);
}
```

## Application Layer Deep Dive

### CQRS Implementation

#### Commands
Represent write operations that change system state:

```csharp
public sealed record GenerateLinkCommand(
    string TargetUrl,
    int LanguageId,
    int ThemeId,
    int FormatId) : ICommand<GenerateLinkResponse>;
```

#### Command Handlers
Execute business logic for commands:

```csharp
internal sealed class GenerateLinkCommandHandler(
    ILinkRepository linkRepository,
    IPhraseGenerator phraseGenerator,
    IUrlMaker urlMaker,
    ResiliencePipelineProvider<string> resiliencePipelineProvider,
    ILogger<GenerateLinkCommandHandler> logger)
    : ICommandHandler<GenerateLinkCommand, GenerateLinkResponse>
{
    public async Task<Result<GenerateLinkResponse>> Handle(
        GenerateLinkCommand request, 
        CancellationToken cancellationToken)
    {
        // 1. Validate input
        // 2. Generate unique phrase with retry
        // 3. Create domain entity
        // 4. Return response
    }
}
```

#### Queries
Represent read operations:

```csharp
public sealed record GetLinkByPhraseQuery(string Phrase) : IQuery<LinkResponse>;
```

#### Query Handlers
Retrieve data without side effects:

```csharp
internal sealed class GetLinkByPhraseQueryHandler(
    ILinkRepository linkRepository)
    : IQueryHandler<GetLinkByPhraseQuery, LinkResponse>
{
    public async Task<Result<LinkResponse>> Handle(
        GetLinkByPhraseQuery request,
        CancellationToken cancellationToken)
    {
        // Retrieve and map data
    }
}
```

### MediatR Pipeline Behaviors

#### Validation Behavior
Automatically validates commands/queries using FluentValidation:

```csharp
public sealed class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Run validators
        // Return validation errors or continue
    }
}
```

#### Logging Behavior
Logs all commands and queries:

```csharp
public sealed class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    // Logs request/response with timing
}
```

### Domain Event Handling

#### Domain Event Handlers
React to domain events within the same transaction:

```csharp
internal sealed class LinkCreatedDomainEventHandler(
    IPublishEndpoint publishEndpoint)
    : IDomainEventHandler<LinkCreatedDomainEvent>
{
    public async Task Handle(
        LinkCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // Publish integration event
        await publishEndpoint.Publish(
            new LinkCreatedIntegrationEvent(notification.LinkId.Value),
            cancellationToken);
    }
}
```

### Integration Events

#### Event Contracts
Defined in `OffndAt.Contracts`:

```csharp
public sealed record LinkVisitedIntegrationEvent
{
    public Guid LinkId { get; init; }
    public DateTime VisitedAtUtc { get; init; }
}
```

#### Event Publishers
Domain event handlers publish to RabbitMQ via MassTransit

#### Event Consumers
Background worker consumes and processes events

## Infrastructure Layer Deep Dive

### Messaging with MassTransit

#### Configuration
```csharp
services.AddMassTransit(x =>
{
    x.AddConsumers(assemblies);
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(settings.Host, settings.VirtualHost, h =>
        {
            h.Username(settings.Username);
            h.Password(settings.Password);
        });
        
        cfg.ConfigureEndpoints(context);
    });
});
```

#### Message Retry Policy
```csharp
cfg.UseMessageRetry(r => r.Incremental(3, 
    TimeSpan.FromSeconds(1), 
    TimeSpan.FromSeconds(2)));
```

### Observability

#### OpenTelemetry
Comprehensive telemetry for metrics, traces, and logs:

```csharp
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation())
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation());
```

#### Serilog
Structured logging with multiple sinks:

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry()
    .CreateLogger();
```

### Caching

#### In-Memory Cache
```csharp
services.AddMemoryCache(options =>
{
    options.SizeLimit = settings.SizeLimit;
    options.CompactionPercentage = settings.CompactionPercentage;
});
```

#### Cache Abstraction
```csharp
public interface ILinkVisitCache
{
    Task<bool> HasVisitedAsync(LinkId linkId, string visitorId);
    Task MarkAsVisitedAsync(LinkId linkId, string visitorId, TimeSpan expiration);
}
```

### Resilience with Polly

#### Retry Policy for Phrase Generation
```csharp
services.AddResiliencePipeline<string, Result<Phrase>>(
    "phrase-retry",
    builder => builder
        .AddRetry(new RetryStrategyOptions<Result<Phrase>>
        {
            MaxRetryAttempts = 5,
            ShouldHandle = new PredicateBuilder<Result<Phrase>>()
                .Handle<Exception>()
                .HandleResult(r => r.IsFailure && r.Error == DomainErrors.Phrase.AlreadyInUse)
        }));
```

## Persistence Layer Deep Dive

### DbContext Configuration

```csharp
public sealed class OffndAtDbContext : DbContext
{
    public DbSet<Link> Links { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish domain events before saving
        await PublishDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### Entity Configurations

```csharp
internal sealed class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Id)
            .HasConversion(id => id.Value, value => new LinkId(value));
        
        builder.OwnsOne(l => l.Phrase, phrase =>
        {
            phrase.Property(p => p.Value).HasColumnName("Phrase");
        });
        
        builder.OwnsOne(l => l.TargetUrl, url =>
        {
            url.Property(u => u.Value).HasColumnName("TargetUrl");
        });
        
        builder.HasIndex(l => l.Phrase).IsUnique();
    }
}
```

### Repository Implementation

```csharp
internal sealed class LinkRepository(OffndAtDbContext context) : ILinkRepository
{
    public async Task<Maybe<Link>> GetByPhraseAsync(
        Phrase phrase, 
        CancellationToken cancellationToken = default)
    {
        var link = await context.Links
            .FirstOrDefaultAsync(l => l.Phrase == phrase, cancellationToken);
        
        return link ?? Maybe<Link>.None;
    }
    
    public void Insert(Link link) => context.Links.Add(link);
}
```

### Specifications Pattern

```csharp
public sealed class LinkByPhraseSpecification : Specification<Link>
{
    private readonly Phrase _phrase;
    
    public LinkByPhraseSpecification(Phrase phrase) => _phrase = phrase;
    
    public override Expression<Func<Link, bool>> ToExpression() 
        => link => link.Phrase == _phrase;
}
```

## API Layer Deep Dive

### Minimal API Endpoints

```csharp
public sealed class LinksEndpoints : IEndpointDefinition
{
    public void DefineEndpoints(IEndpointRouteBuilder app)
    {
        var links = app.MapGroup("/api/v1/links")
            .WithTags("Links")
            .RequireRateLimiting("fixed");
        
        links.MapPost("/", GenerateLink)
            .WithName("GenerateLink")
            .Produces<GenerateLinkResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        
        links.MapGet("/{phrase}", GetLinkByPhrase)
            .WithName("GetLinkByPhrase")
            .Produces<LinkResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
    
    private static async Task<IResult> GenerateLink(
        GenerateLinkRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GenerateLinkCommand(/* ... */);
        var result = await sender.Send(command, cancellationToken);
        
        return result.IsSuccess
            ? Results.Created($"/api/v1/links/{result.Value.Phrase}", result.Value)
            : result.ToProblemDetails();
    }
}
```

### Middleware Pipeline

```csharp
app.UseForwardedHeaders()
   .UseResponseCompression()
   .UseRateLimiter()
   .UseCors()
   .UseAuthentication()
   .UseAuthorization()
   .UseCustomExceptionHandler()
   .UseHttpsRedirection();
```

### Exception Handling

```csharp
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception occurred");
            await HandleExceptionAsync(context, exception);
        }
    }
}
```

## Dependency Injection

### Service Registration Pattern

Each layer has a `DependencyInjectionExtensions.cs` file:

```csharp
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
```

### Registration in Program.cs

```csharp
builder.Services
    .AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);
```

## Testing Architecture

### Unit Tests
- Test domain logic in isolation
- Use Bogus for test data generation
- FluentAssertions for readable assertions

### Integration Tests
- Use Testcontainers for PostgreSQL and RabbitMQ
- Test repository operations
- Verify database configurations

### Functional Tests
- End-to-end API testing
- WebApplicationFactory for in-memory hosting
- Complete workflow validation
