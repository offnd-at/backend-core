# Repository Pattern

## Overview

The Repository Pattern abstracts data access logic, providing a collection-like interface for accessing domain objects. It sits between the domain and data mapping layers, acting as an in-memory collection of domain objects.

## Purpose

- Abstract data access from business logic
- Centralize data access logic
- Enable testability through mocking
- Provide a consistent API for data operations

## Implementation

### Repository Interface (Domain Layer)

```csharp
namespace OffndAt.Domain.Repositories;

public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
    Task<bool> HasAnyWithPhraseAsync(Phrase phrase, CancellationToken cancellationToken = default);
    void Insert(Link link);
}
```

**Key Points**:
- Defined in **Domain layer** (dependency inversion)
- Works with domain entities, not DTOs
- One repository per aggregate root
- Returns `Maybe<T>` for optional results

### Repository Implementation (Persistence Layer)

```csharp
namespace OffndAt.Persistence.Repositories;

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
    
    public async Task<bool> HasAnyWithPhraseAsync(
        Phrase phrase,
        CancellationToken cancellationToken = default)
    {
        return await context.Links
            .AnyAsync(l => l.Phrase == phrase, cancellationToken);
    }
    
    public void Insert(Link link)
    {
        context.Links.Add(link);
    }
}
```

**Key Points**:
- Implemented in **Persistence layer**
- Uses EF Core DbContext
- Sealed and internal (not exposed outside persistence)
- Simple, focused methods

## Design Principles

### 1. One Repository Per Aggregate Root

```csharp
// ✅ GOOD: Repository for aggregate root
public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase);
    void Insert(Link link);
}

// ❌ BAD: Repository for every entity
public interface IVisitRepository { }  // Visit is part of Link aggregate
```

### 2. Collection-Like Interface

```csharp
public interface ILinkRepository
{
    // Like a collection
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase);
    void Insert(Link link);
    
    // NOT like a database
    // ❌ void SaveChanges();
    // ❌ void BeginTransaction();
}
```

### 3. Return Domain Objects

```csharp
// ✅ GOOD: Returns domain entity
Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase);

// ❌ BAD: Returns DTO
Task<LinkDto> GetByPhraseAsync(string phrase);
```

### 4. Synchronous Writes, Asynchronous Reads

```csharp
public interface ILinkRepository
{
    // Reads are async (I/O bound)
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase, CancellationToken cancellationToken);
    
    // Writes are sync (just tracking changes)
    void Insert(Link link);
    void Update(Link link);
    void Delete(Link link);
}
```

## Usage in Application Layer

### Command Handler

```csharp
internal sealed class GenerateLinkCommandHandler(
    ILinkRepository linkRepository,
    IPhraseGenerator phraseGenerator)
    : ICommandHandler<GenerateLinkCommand, GenerateLinkResponse>
{
    public async Task<Result<GenerateLinkResponse>> Handle(
        GenerateLinkCommand request,
        CancellationToken cancellationToken)
    {
        // Generate phrase
        var phraseResult = await phraseGenerator.GenerateAsync(/* ... */);
        
        // Check if phrase exists
        var exists = await linkRepository.HasAnyWithPhraseAsync(phraseResult.Value, cancellationToken);
        if (exists)
            return Result.Failure<GenerateLinkResponse>(DomainErrors.Phrase.AlreadyInUse);
        
        // Create and insert link
        var link = Link.Create(/* ... */);
        linkRepository.Insert(link);  // Just tracks the change
        
        // SaveChanges called by UnitOfWork/DbContext
        return Result.Success(new GenerateLinkResponse { /* ... */ });
    }
}
```

### Query Handler

```csharp
internal sealed class GetLinkByPhraseQueryHandler(
    ILinkRepository linkRepository)
    : IQueryHandler<GetLinkByPhraseQuery, LinkResponse>
{
    public async Task<Result<LinkResponse>> Handle(
        GetLinkByPhraseQuery request,
        CancellationToken cancellationToken)
    {
        var phraseResult = Phrase.Create(request.Phrase);
        if (phraseResult.IsFailure)
            return Result.Failure<LinkResponse>(phraseResult.Error);
        
        var maybeLink = await linkRepository.GetByPhraseAsync(phraseResult.Value, cancellationToken);
        
        return maybeLink.HasValue
            ? Result.Success(MapToResponse(maybeLink.Value))
            : Result.Failure<LinkResponse>(DomainErrors.Link.NotFound);
    }
}
```

## Advanced Patterns

### Specification Pattern

Encapsulate query logic:

```csharp
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    
    public bool IsSatisfiedBy(T entity) => ToExpression().Compile()(entity);
}

public sealed class LinkByPhraseSpecification : Specification<Link>
{
    private readonly Phrase _phrase;
    
    public LinkByPhraseSpecification(Phrase phrase) => _phrase = phrase;
    
    public override Expression<Func<Link, bool>> ToExpression() =>
        link => link.Phrase == _phrase;
}

// Usage in repository
public async Task<Maybe<Link>> GetBySpecificationAsync(
    Specification<Link> specification,
    CancellationToken cancellationToken = default)
{
    var link = await context.Links
        .Where(specification.ToExpression())
        .FirstOrDefaultAsync(cancellationToken);
    
    return link ?? Maybe<Link>.None;
}
```

### Generic Repository Base

```csharp
public abstract class Repository<TEntity, TEntityId>
    where TEntity : AggregateRoot<TEntityId>
    where TEntityId : EntityId
{
    protected readonly DbContext Context;
    
    protected Repository(DbContext context) => Context = context;
    
    public virtual async Task<Maybe<TEntity>> GetByIdAsync(
        TEntityId id,
        CancellationToken cancellationToken = default)
    {
        var entity = await Context.Set<TEntity>()
            .FindAsync([id], cancellationToken);
        
        return entity ?? Maybe<TEntity>.None;
    }
    
    public virtual void Insert(TEntity entity) => Context.Set<TEntity>().Add(entity);
    
    public virtual void Delete(TEntity entity) => Context.Set<TEntity>().Remove(entity);
}

// Specific repository
internal sealed class LinkRepository : Repository<Link, LinkId>, ILinkRepository
{
    public LinkRepository(OffndAtDbContext context) : base(context) { }
    
    public async Task<Maybe<Link>> GetByPhraseAsync(
        Phrase phrase,
        CancellationToken cancellationToken = default)
    {
        var link = await Context.Links
            .FirstOrDefaultAsync(l => l.Phrase == phrase, cancellationToken);
        
        return link ?? Maybe<Link>.None;
    }
}
```

## Unit of Work Pattern

DbContext acts as Unit of Work:

```csharp
public sealed class OffndAtDbContext : DbContext
{
    public DbSet<Link> Links { get; set; }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Publish domain events before saving
        await PublishDomainEventsAsync(cancellationToken);
        
        // Save all changes in a transaction
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker.Entries<IAggregateRoot>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();
        
        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
        
        ChangeTracker.Entries<IAggregateRoot>()
            .ToList()
            .ForEach(entry => entry.Entity.ClearDomainEvents());
    }
}
```

## Testing

### Mocking Repositories

```csharp
[Test]
public async Task Handle_ExistingPhrase_ReturnsFailure()
{
    // Arrange
    var mockRepository = Substitute.For<ILinkRepository>();
    mockRepository.HasAnyWithPhraseAsync(Arg.Any<Phrase>())
        .Returns(true);  // Phrase exists
    
    var handler = new GenerateLinkCommandHandler(mockRepository, /* ... */);
    var command = new GenerateLinkCommand(/* ... */);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(DomainErrors.Phrase.AlreadyInUse);
}
```

### Integration Tests

```csharp
[Test]
public async Task GetByPhraseAsync_ExistingPhrase_ReturnsLink()
{
    // Arrange
    await using var context = CreateDbContext();
    var repository = new LinkRepository(context);
    
    var link = Link.Create(/* ... */);
    context.Links.Add(link);
    await context.SaveChangesAsync();
    
    // Act
    var result = await repository.GetByPhraseAsync(link.Phrase);
    
    // Assert
    result.HasValue.Should().BeTrue();
    result.Value.Phrase.Should().Be(link.Phrase);
}
```

## Benefits

### ✅ Testability

Mock repositories for unit tests:

```csharp
var mockRepository = Substitute.For<ILinkRepository>();
mockRepository.GetByPhraseAsync(Arg.Any<Phrase>())
    .Returns(Maybe<Link>.From(testLink));
```

### ✅ Centralized Data Access

All data access logic in one place:

```csharp
// All Link queries go through ILinkRepository
var link = await _linkRepository.GetByPhraseAsync(phrase);
```

### ✅ Database Independence

Swap implementations without changing business logic:

```csharp
// EF Core implementation
services.AddScoped<ILinkRepository, EfCoreLinkRepository>();

// Dapper implementation
services.AddScoped<ILinkRepository, DapperLinkRepository>();
```

### ✅ Consistency

Consistent API across different aggregates:

```csharp
public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase);
    void Insert(Link link);
}

public interface IUserRepository
{
    Task<Maybe<User>> GetByEmailAsync(Email email);
    void Insert(User user);
}
```

## Anti-Patterns

### ❌ Generic Repository for Everything

```csharp
// BAD: Too generic, loses domain meaning
public interface IRepository<T>
{
    Task<T> GetByIdAsync(Guid id);
    void Insert(T entity);
    void Update(T entity);
    void Delete(T entity);
}

// GOOD: Domain-specific methods
public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase);
    void Insert(Link link);
}
```

### ❌ Exposing IQueryable

```csharp
// BAD: Leaks data access concerns
public interface ILinkRepository
{
    IQueryable<Link> GetAll();
}

// GOOD: Specific queries
public interface ILinkRepository
{
    Task<IReadOnlyList<Link>> GetRecentLinksAsync(int count);
}
```

### ❌ Repository for Non-Aggregates

```csharp
// BAD: Repository for value object
public interface IPhraseRepository { }

// GOOD: Phrase is part of Link aggregate
public interface ILinkRepository
{
    Task<Maybe<Link>> GetByPhraseAsync(Phrase phrase);
}
```

## Related Patterns

- [Clean Architecture](./clean-architecture.md) - Repositories enable dependency inversion
- [Domain-Driven Design](./domain-driven-design.md) - One repository per aggregate
- [CQRS](./cqrs.md) - Repositories used by commands and queries
- [Dependency Injection](./dependency-injection.md) - Repositories injected into handlers

## Further Reading

- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html) by Martin Fowler
- [Implementing the Repository and Unit of Work Patterns](https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application) by Microsoft
