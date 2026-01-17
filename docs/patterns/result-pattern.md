# Result Pattern

## Overview

The Result Pattern is a functional programming pattern that provides explicit error handling without using exceptions for control flow. It wraps operation outcomes in a `Result<T>` type that can represent either success with a value or failure with an error.

## Problem

Traditional exception-based error handling has issues:
- ❌ Exceptions are expensive (performance)
- ❌ Exceptions for control flow are anti-pattern
- ❌ No compile-time guarantee of error handling
- ❌ Hidden control flow paths

## Solution

Return a `Result<T>` that explicitly represents success or failure:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public Error Error { get; }
}
```

## Implementation

### Result Type

```csharp
namespace OffndAt.Domain.Core.Primitives;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            throw new InvalidOperationException();
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    
    public static Result Success() => new(true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
}
```

### Generic Result

```csharp
public class Result<TValue> : Result
{
    private readonly TValue _value;
    
    protected internal Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }
    
    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access value of a failure result");
}
```

### Error Type

```csharp
public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    
    public static implicit operator Result(Error error) => Result.Failure(error);
}
```

## Usage Examples

### Creating Results

```csharp
// Success
var successResult = Result.Success();
var successWithValue = Result.Success(new Link(/* ... */));

// Failure
var failureResult = Result.Failure(DomainErrors.Link.NotFound);
var failureWithType = Result.Failure<Link>(DomainErrors.Link.NotFound);
```

### Value Object Creation

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

### Command Handler

```csharp
public async Task<Result<GenerateLinkResponse>> Handle(
    GenerateLinkCommand request,
    CancellationToken cancellationToken)
{
    // Validate URL
    var targetUrlResult = Url.Create(request.TargetUrl);
    if (targetUrlResult.IsFailure)
        return Result.Failure<GenerateLinkResponse>(targetUrlResult.Error);
    
    // Validate language
    var maybeLanguage = Language.FromValue(request.LanguageId);
    if (maybeLanguage.HasNoValue)
        return Result.Failure<GenerateLinkResponse>(DomainErrors.Language.NotFound);
    
    // Generate phrase
    var phraseResult = await _phraseGenerator.GenerateAsync(/* ... */);
    if (phraseResult.IsFailure)
        return Result.Failure<GenerateLinkResponse>(phraseResult.Error);
    
    // Create link
    var link = Link.Create(phraseResult.Value, targetUrlResult.Value, maybeLanguage.Value, maybeTheme.Value);
    _linkRepository.Insert(link);
    
    return Result.Success(new GenerateLinkResponse { /* ... */ });
}
```

## Railway-Oriented Programming

Chain operations that can fail:

```csharp
public static class ResultExtensions
{
    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Error error)
    {
        if (result.IsFailure)
            return result;
        
        return predicate(result.Value)
            ? result
            : Result.Failure<T>(error);
    }
    
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value))
            : Result.Failure<TOut>(result.Error);
    }
    
    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> func)
    {
        return result.IsSuccess
            ? await func(result.Value)
            : Result.Failure<TOut>(result.Error);
    }
}
```

### Chaining Example

```csharp
public static Result<Phrase> Create(string phrase) =>
    Result.Create(phrase, DomainErrors.Phrase.NullOrEmpty)
        .Ensure(value => !string.IsNullOrWhiteSpace(value), DomainErrors.Phrase.NullOrEmpty)
        .Ensure(value => value.Length <= MaxLength, DomainErrors.Phrase.LongerThanAllowed)
        .Map(value => new Phrase(value));
```

## Domain Errors

Centralized error definitions:

```csharp
public static class DomainErrors
{
    public static class Link
    {
        public static readonly Error NotFound = new(
            "Link.NotFound",
            "The link with the specified phrase was not found");
        
        public static readonly Error CouldNotGenerate = new(
            "Link.CouldNotGenerate",
            "Could not generate a unique link after multiple attempts");
    }
    
    public static class Phrase
    {
        public static readonly Error NullOrEmpty = new(
            "Phrase.NullOrEmpty",
            "The phrase is required");
        
        public static readonly Error LongerThanAllowed = new(
            "Phrase.LongerThanAllowed",
            $"The phrase is longer than {Phrase.MaxLength} characters");
        
        public static readonly Error AlreadyInUse = new(
            "Phrase.AlreadyInUse",
            "The phrase is already in use");
    }
    
    public static class Url
    {
        public static readonly Error NullOrEmpty = new(
            "Url.NullOrEmpty",
            "The URL is required");
        
        public static readonly Error LongerThanAllowed = new(
            "Url.LongerThanAllowed",
            $"The URL is longer than {Url.MaxLength} characters");
    }
}
```

## API Integration

Convert results to HTTP responses:

```csharp
public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to problem details");
        
        return Results.Problem(
            statusCode: GetStatusCode(result.Error),
            title: "An error occurred",
            detail: result.Error.Message,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = result.Error.Code
            });
    }
    
    private static int GetStatusCode(Error error) => error.Code switch
    {
        _ when error.Code.EndsWith(".NotFound") => StatusCodes.Status404NotFound,
        _ when error.Code.EndsWith(".AlreadyExists") => StatusCodes.Status409Conflict,
        _ when error.Code.Contains(".Validation") => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
}
```

### Endpoint Usage

```csharp
public static async Task<IResult> GenerateLink(
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
```

## Benefits

### ✅ Explicit Error Handling

Errors are part of the method signature:

```csharp
// Clear that this can fail
public async Task<Result<Link>> GetLinkAsync(Phrase phrase)
{
    var link = await _repository.GetByPhraseAsync(phrase);
    return link.HasValue
        ? Result.Success(link.Value)
        : Result.Failure<Link>(DomainErrors.Link.NotFound);
}
```

### ✅ Type Safety

Compile-time guarantee of error handling:

```csharp
var result = await GetLinkAsync(phrase);

// Must check IsSuccess before accessing Value
if (result.IsSuccess)
{
    var link = result.Value;  // ✅ Safe
}
else
{
    var error = result.Error;  // ✅ Handle error
}
```

### ✅ Performance

No exception overhead for expected failures:

```csharp
// Fast: No exception thrown
return Result.Failure<Link>(DomainErrors.Link.NotFound);

// Slow: Exception overhead
throw new NotFoundException("Link not found");
```

### ✅ Composability

Chain operations elegantly:

```csharp
return await GetLinkAsync(phrase)
    .Bind(link => ValidateLinkAsync(link))
    .Map(link => MapToResponse(link));
```

## Common Patterns

### First Failure

Return first failure or success:

```csharp
public static Result FirstFailureOrSuccess(params Result[] results)
{
    foreach (var result in results)
    {
        if (result.IsFailure)
            return result;
    }
    
    return Success();
}

// Usage
var redirectUrl = _urlMaker.MakeRedirectUrlForPhrase(phrase);
var statsUrl = _urlMaker.MakeStatsUrlForPhrase(phrase);
var result = Result.FirstFailureOrSuccess(redirectUrl, statsUrl);
```

### Maybe Type

Represent optional values:

```csharp
public sealed class Maybe<T>
{
    private readonly T _value;
    
    public bool HasValue { get; }
    public bool HasNoValue => !HasValue;
    
    public T Value => HasValue
        ? _value
        : throw new InvalidOperationException("Maybe has no value");
    
    public static Maybe<T> From(T value) => new(value);
    public static Maybe<T> None => new();
}

// Usage
var maybeLink = await _repository.GetByPhraseAsync(phrase);
if (maybeLink.HasValue)
{
    return Result.Success(maybeLink.Value);
}
```

## Testing

```csharp
[Test]
public async Task Handle_InvalidUrl_ReturnsFailure()
{
    // Arrange
    var handler = new GenerateLinkCommandHandler(/* ... */);
    var command = new GenerateLinkCommand("invalid-url", 1, 1, 1);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Code.Should().Be("Url.Invalid");
}

[Test]
public async Task Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var handler = new GenerateLinkCommandHandler(/* ... */);
    var command = new GenerateLinkCommand("https://example.com", 1, 1, 1);
    
    // Act
    var result = await handler.Handle(command, CancellationToken.None);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeNull();
}
```

## Anti-Patterns

### ❌ Throwing Exceptions from Result

```csharp
// BAD: Defeats the purpose
if (result.IsFailure)
    throw new Exception(result.Error.Message);

// GOOD: Handle the error
if (result.IsFailure)
    return result.ToProblemDetails();
```

### ❌ Ignoring Failures

```csharp
// BAD: Ignoring potential failure
var link = GetLinkAsync(phrase).Result.Value;

// GOOD: Check before accessing
var result = await GetLinkAsync(phrase);
if (result.IsSuccess)
{
    var link = result.Value;
}
```

## Related Patterns

- [CQRS](./cqrs.md) - Commands and queries return Results
- [Domain-Driven Design](./domain-driven-design.md) - Value objects use Results
- [Clean Architecture](./clean-architecture.md) - Results flow through layers

## Further Reading

- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) by Scott Wlaschin
- [Functional C#](https://www.manning.com/books/functional-programming-in-c-sharp) by Enrico Buonanno
