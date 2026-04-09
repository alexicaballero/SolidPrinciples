# Domain Patterns Reference

## Entity Base Classes

### AuditableEntity

```csharp
namespace SolidPrinciples.Common;

public abstract class AuditableEntity
{
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ModifiedAt { get; protected set; }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

### Result Pattern

```csharp
namespace SolidPrinciples.Common;

public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default!, false, error);
}

public class Result<T> : Result
{
    private readonly T _value;

    protected internal Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access Value on a failed result.");
}
```

### Error Record

```csharp
namespace SolidPrinciples.Common;

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}
```

### Domain Event Interface

```csharp
namespace SolidPrinciples.Common;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredAt { get; }
}

public abstract record DomainEventBase(Guid Id) : IDomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
```

## Domain Entity Template

```csharp
namespace SolidPrinciples.{Principle}.Solution;

/// <summary>
/// Represents a {entity description}.
/// </summary>
public sealed class {EntityName} : AuditableEntity
{
    public Guid Id { get; private set; }
    // Properties with private setters

    private {EntityName}() { } // EF Core constructor

    /// <summary>
    /// Factory method to create a new {entity}.
    /// </summary>
    public static Result<{EntityName}> Create(/* parameters */)
    {
        // Validation
        // Construction
        // Raise domain events
        return Result.Success(entity);
    }
}
```

## Repository Interface Template

```csharp
namespace SolidPrinciples.{Principle}.Solution;

public interface I{EntityName}Repository
{
    Task<{EntityName}?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add({EntityName} entity);
}
```

## Command Handler Template

```csharp
namespace SolidPrinciples.{Principle}.Solution;

public sealed class {Action}{Entity}CommandHandler : ICommandHandler<{Action}{Entity}Command, Guid>
{
    private readonly I{Entity}Repository _{entity}Repository;
    private readonly IUnitOfWork _unitOfWork;

    public {Action}{Entity}CommandHandler(
        I{Entity}Repository {entity}Repository,
        IUnitOfWork unitOfWork)
    {
        _{entity}Repository = {entity}Repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(
        {Action}{Entity}Command command,
        CancellationToken ct = default)
    {
        // 1. Validate input
        // 2. Execute domain logic
        // 3. Persist changes
        // 4. Return result
    }
}
```
