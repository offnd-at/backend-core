# Architectural Patterns

This document provides an overview of the key architectural patterns used in the offnd.at backend system.

## Pattern Overview

The system employs several complementary patterns that work together to create a maintainable, scalable, and testable architecture:

```mermaid
graph TD
    CA[Clean Architecture] --> DDD[Domain-Driven Design]
    CA --> CQRS[CQRS Pattern]
    DDD --> RP[Repository Pattern]
    DDD --> EDA[Event-Driven Architecture]
    CQRS --> RP
    CQRS --> Result[Result Pattern]
    EDA --> Result
    
    style CA fill:#4CAF50
    style DDD fill:#2196F3
    style CQRS fill:#FF9800
```

## 1. Clean Architecture

**Purpose**: Organize code into layers with clear dependency rules

**Key Principles**:
- Dependencies point inward toward the domain
- Inner layers have no knowledge of outer layers
- Business logic is independent of frameworks

**Implementation**:
- **Domain Layer**: Core business rules (no dependencies)
- **Application Layer**: Use cases and workflows (depends on Domain)
- **Infrastructure Layer**: External services (depends on Application)
- **Presentation Layer**: API endpoints (depends on Application)

**Benefits**:
- ✅ Testable business logic
- ✅ Framework independence
- ✅ Clear separation of concerns
- ✅ Easy to understand and navigate

---

## 2. Domain-Driven Design (DDD)

**Purpose**: Model complex business domains with rich, expressive code

**Key Concepts**:
- **Entities**: Objects with identity (e.g., `Link`)
- **Value Objects**: Immutable objects defined by attributes (e.g., `Url`, `Phrase`)
- **Aggregates**: Consistency boundaries (e.g., `Link` aggregate)
- **Domain Events**: Capture domain occurrences (e.g., `LinkCreatedDomainEvent`)
- **Ubiquitous Language**: Shared vocabulary between developers and domain experts

**Implementation**:
```csharp
// Aggregate Root
public sealed class Link : AggregateRoot<LinkId>
{
    public Phrase Phrase { get; }
    public Url TargetUrl { get; }
    public Language Language { get; }
    public Theme Theme { get; }
    
    public static Link Create(Phrase phrase, Url targetUrl, Language language, Theme theme)
    {
        var link = new Link(phrase, targetUrl, language, theme);
        link.RaiseDomainEvent(new LinkCreatedDomainEvent(link.Id, link.Language, link.Theme));
        return link;
    }
}

// Value Object
public sealed record Phrase
{
    public string Value { get; private init; }
    
    public static Result<Phrase> Create(string value)
    {
        // Validation logic
        return new Phrase { Value = value };
    }
}
```

**Benefits**:
- ✅ Rich domain model
- ✅ Encapsulated business rules
- ✅ Clear business intent
- ✅ Reduced coupling

---

## 3. CQRS (Command Query Responsibility Segregation)

**Purpose**: Separate read and write operations for clarity and optimization

**Key Concepts**:
- **Commands**: Write operations that change state
- **Queries**: Read operations that return data
- **Handlers**: Execute commands and queries
- **Separation**: Different models for reads and writes

**Implementation**:
```csharp
// Command
public sealed record GenerateLinkCommand(
    string TargetUrl,
    int LanguageId,
    int ThemeId,
    int FormatId) : ICommand<GenerateLinkResponse>;

// Command Handler
internal sealed class GenerateLinkCommandHandler 
    : ICommandHandler<GenerateLinkCommand, GenerateLinkResponse>
{
    public async Task<Result<GenerateLinkResponse>> Handle(
        GenerateLinkCommand request,
        CancellationToken cancellationToken)
    {
        // Business logic
    }
}

// Query
public sealed record GetLinkByPhraseQuery(string Phrase) : IQuery<LinkResponse>;

// Query Handler
internal sealed class GetLinkByPhraseQueryHandler 
    : IQueryHandler<GetLinkByPhraseQuery, LinkResponse>
{
    public async Task<Result<LinkResponse>> Handle(
        GetLinkByPhraseQuery request,
        CancellationToken cancellationToken)
    {
        // Data retrieval
    }
}
```

**Benefits**:
- ✅ Clear intent (read vs. write)
- ✅ Optimized for different concerns
- ✅ Scalable (can scale reads and writes independently)
- ✅ Easier to maintain

---

## 4. Result Pattern

**Purpose**: Explicit error handling without exceptions for control flow

**Key Concepts**:
- **Result<T>**: Represents success or failure
- **Error**: Strongly-typed error information
- **Railway-Oriented Programming**: Chain operations that can fail

**Implementation**:
```csharp
// Result type
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }
    
    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static Result<T> Failure(Error error) => new(default, false, error);
}

// Usage
public async Task<Result<Link>> GetLinkAsync(Phrase phrase)
{
    var link = await _repository.GetByPhraseAsync(phrase);
    
    return link.HasValue
        ? Result.Success(link.Value)
        : Result.Failure<Link>(DomainErrors.Link.NotFound);
}
```

**Benefits**:
- ✅ Explicit error handling
- ✅ Type-safe errors
- ✅ Better performance (no exceptions)
- ✅ Composable operations

---

## 5. Event-Driven Architecture

**Purpose**: Decouple components through asynchronous event communication

**Key Concepts**:
- **Domain Events**: In-process events within the same transaction
- **Integration Events**: Cross-service events via message bus
- **Event Handlers**: React to events
- **Eventual Consistency**: Accept temporary inconsistency for scalability

**Implementation**:
```csharp
// Domain Event
public sealed record LinkCreatedDomainEvent(LinkId LinkId) : IDomainEvent;

// Domain Event Handler
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

// Integration Event
public sealed record LinkCreatedIntegrationEvent
{
    public Guid LinkId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

// Integration Event Consumer
public sealed class LinkCreatedIntegrationEventConsumer 
    : IConsumer<LinkCreatedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<LinkCreatedIntegrationEvent> context)
    {
        // Process event
    }
}
```

**Event Flow**:
1. Aggregate raises domain event
2. Domain event handler executes (same transaction)
3. Integration event published to RabbitMQ
4. Background worker consumes integration event
5. Asynchronous processing completes

**Benefits**:
- ✅ Loose coupling
- ✅ Scalability
- ✅ Asynchronous processing
- ✅ Audit trail
