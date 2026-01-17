# Domain-Driven Design (DDD) Pattern

## Overview

Domain-Driven Design is an approach to software development that emphasizes collaboration between technical and domain experts to create a model that accurately reflects the business domain.

## Core Concepts

### Ubiquitous Language

A shared vocabulary between developers and domain experts used consistently in code and conversation.

**Examples in offnd.at**:
- **Link**: A short URL with a profane phrase
- **Phrase**: The memorable, profane identifier
- **Target URL**: The destination URL
- **Visit**: When someone follows a short link

### Building Blocks

#### 1. Entities

Objects with identity that persists over time:

```csharp
public sealed class Link : AggregateRoot<LinkId>
{
    public LinkId Id { get; } // Identity
    public Phrase Phrase { get; }
    public Url TargetUrl { get; }
    public Language Language { get; }
    public Theme Theme { get; }
    
    // Behavior encapsulated in entity
    public void RecordVisit()
    {
        RaiseDomainEvent(new LinkVisitedDomainEvent(Id, new LinkVisitedContext(Language, Theme, DateTimeOffset.UtcNow)));
    }
}
```

**Characteristics**:
- Has unique identity (`LinkId`)
- Identity persists across changes
- Encapsulates behavior
- Maintains invariants

#### 2. Value Objects

Immutable objects defined by their attributes:

```csharp
public sealed record Phrase
{
    public string Value { get; }
    
    private Phrase(string value) => Value = value;
    
    public static Result<Phrase> Create(string phrase) =>
        Result.Create(phrase, DomainErrors.Phrase.NullOrEmpty)
            .Ensure(value => !string.IsNullOrWhiteSpace(value), DomainErrors.Phrase.NullOrEmpty)
            .Ensure(value => value.Length <= MaxLength, DomainErrors.Phrase.LongerThanAllowed)
            .Map(value => new Phrase(value));
}
```

**Characteristics**:
- Immutable (record type)
- No identity
- Validation in factory method
- Equality by value

#### 3. Aggregates

Cluster of entities and value objects with a root entity:

```csharp
// Link is the Aggregate Root
public sealed class Link : AggregateRoot<LinkId>
{
    // Aggregate members
    public Phrase Phrase { get; }      // Value Object
    public Url TargetUrl { get; }      // Value Object
    public Language Language { get; }  // Enumeration
    public Theme Theme { get; }        // Enumeration
    
    // Factory method ensures invariants
    public static Link Create(Phrase phrase, Url targetUrl, Language language, Theme theme)
    {
        // Validation happens in constructor
        var link = new Link(phrase, targetUrl, language, theme);
        link.RaiseDomainEvent(new LinkCreatedDomainEvent(link.Id, link.Language, link.Theme));
        return link;
    }
}
```

**Rules**:
- External objects can only reference the root
- Invariants enforced within aggregate boundary
- Transactions should not span multiple aggregates
- One repository per aggregate root

#### 4. Domain Events

Capture domain occurrences:

```csharp
public sealed record LinkCreatedDomainEvent(LinkId LinkId, Language Language, Theme Theme) : IDomainEvent;

public sealed record LinkVisitedDomainEvent(LinkId LinkId, LinkVisitedContext Context) : IDomainEvent;
```

**Usage**:
```csharp
public static Link Create(Phrase phrase, Url targetUrl, Language language, Theme theme)
{
    var link = new Link(phrase, targetUrl, language, theme);
    link.RaiseDomainEvent(new LinkCreatedDomainEvent(link.Id, link.Language, link.Theme));
    return link;
}
```

#### 5. Domain Services

Operations that don't naturally belong to an entity:

```csharp
namespace OffndAt.Domain.Services;

public interface ILinkService
{
    Task<Result<Link>> CreateUniqueLink(
        Url targetUrl,
        Language language,
        Theme theme,
        Format format,
        CancellationToken cancellationToken);
}
```

#### 6. Repositories

Abstraction for data access:

```csharp
namespace OffndAt.Domain.Repositories;

public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
    Task<bool> HasAnyWithPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
    void Insert(Link link);
}
```

## Tactical Patterns

### Strongly-Typed IDs

```csharp
public sealed record LinkId(Guid Value) : EntityId(Value);
```

**Benefits**:
- Type safety (can't mix up different IDs)
- Self-documenting code
- Compile-time checks

### Smart Enumerations

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
    
    public static Maybe<Language> FromValue(int value) => 
        GetAll().FirstOrDefault(l => l.Value == value);
}
```

**Benefits**:
- Rich behavior
- Type-safe
- Extensible
- Better than C# enums

### Specifications

```csharp
public sealed class LinkByPhraseSpecification : Specification<Link>
{
    private readonly Phrase _phrase;
    
    public LinkByPhraseSpecification(Phrase phrase) => _phrase = phrase;
    
    public override Expression<Func<Link, bool>> ToExpression() => 
        link => link.Phrase == _phrase;
}
```

## Strategic Patterns

### Bounded Context

The offnd.at system has a single bounded context focused on link management:

```
┌─────────────────────────────────────┐
│      Link Management Context       │
│                                     │
│  - Link Creation                    │
│  - Phrase Generation                │
│  - URL Redirection                  │
│  - Visit Tracking                   │
└─────────────────────────────────────┘
```

### Context Mapping

If the system grows, contexts could be:

```
┌──────────────┐      ┌──────────────┐
│    Links     │◄────►│  Analytics   │
│   Context    │      │   Context    │
└──────────────┘      └──────────────┘
       │
       │ Shared Kernel
       ▼
┌──────────────┐
│   Phrases    │
│   Context    │
└──────────────┘
```

## Best Practices

### ✅ Encapsulation

```csharp
// GOOD: Private setters, factory method
public sealed class Link : AggregateRoot<LinkId>
{
    public Phrase Phrase { get; }  // No setter
    
    private Link(Phrase phrase, Url targetUrl, Language language, Theme theme)
    {
        // Validation in constructor
        Guard.AgainstEmpty(phrase, "The phrase is required.");
        // ...
    }
    
    public static Link Create(/* ... */)
    {
        return new Link(/* ... */);
    }
}
```

### ✅ Invariant Protection

```csharp
// Invariants enforced in constructor
private Link(Phrase phrase, Url targetUrl, Language language, Theme theme)
{
    Guard.AgainstEmpty(phrase, "The phrase is required.");
    Guard.AgainstEmpty(targetUrl, "The target URL is required.");
    Guard.AgainstNull(language, "The language is required.");
    Guard.AgainstNull(theme, "The theme is required.");
    
    Phrase = phrase;
    TargetUrl = targetUrl;
    Language = language;
    Theme = theme;
}
```

### ✅ Rich Domain Model

```csharp
// Behavior in domain entities
public void RecordVisit()
{
    RaiseDomainEvent(new LinkVisitedDomainEvent(Id, new LinkVisitedContext(Language, Theme, DateTimeOffset.UtcNow)));
}
```

## Anti-Patterns

### ❌ Anemic Domain Model

```csharp
// BAD: No behavior, just data
public class Link
{
    public Guid Id { get; set; }
    public string Phrase { get; set; }
    public string TargetUrl { get; set; }
}
```

### ❌ Leaking Domain Logic

```csharp
// BAD: Domain logic in application layer
public class GenerateLinkCommandHandler
{
    public async Task<Result> Handle(GenerateLinkCommand command)
    {
        // Domain logic should be in Link entity
        if (string.IsNullOrEmpty(command.Phrase))
            return Result.Failure(/* ... */);
    }
}
```

### ❌ Exposing Internal State

```csharp
// BAD: Public setters
public class Link
{
    public Phrase Phrase { get; set; }  // Should be { get; }
}
```

## Benefits

✅ **Shared Understanding**: Ubiquitous language bridges technical and business teams  
✅ **Maintainability**: Business logic centralized in domain  
✅ **Flexibility**: Easy to change business rules  
✅ **Testability**: Pure domain logic is easy to test  
✅ **Scalability**: Clear boundaries enable decomposition  

## Related Patterns

- [Clean Architecture](./clean-architecture.md) - Architectural foundation
- [CQRS](./cqrs.md) - Separates reads and writes
- [Event-Driven Architecture](./event-driven-architecture.md) - Domain events
- [Repository Pattern](./repository-pattern.md) - Data access abstraction

## Further Reading

- [Domain-Driven Design: Tackling Complexity in the Heart of Software](https://www.amazon.com/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215) by Eric Evans
- [Implementing Domain-Driven Design](https://www.amazon.com/Implementing-Domain-Driven-Design-Vaughn-Vernon/dp/0321834577) by Vaughn Vernon
