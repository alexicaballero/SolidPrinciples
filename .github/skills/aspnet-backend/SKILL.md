---
name: aspnet-backend
description: 'ASP.NET Core backend development skill for SOLID principles project. Use when: creating C# class libraries, domain entities, repositories, command handlers, unit tests, Result pattern, Clean Architecture layers, dependency injection, factory methods, domain events, or any backend code following Gathering reference patterns.'
argument-hint: "Describe the backend component to build (e.g., 'Create Session entity with factory method')"
---

# ASP.NET Core Backend Development

## When to Use

- Creating or modifying C# class library projects
- Implementing domain entities, value objects, or aggregates
- Building repository interfaces and implementations
- Writing command/query handlers (CQRS pattern)
- Configuring dependency injection
- Implementing the Result pattern for error handling
- Writing unit tests with xUnit, FluentAssertions, NSubstitute
- Any backend code that follows Clean Architecture

## Technology Stack

- .NET 9 / C# 12
- xUnit (testing), FluentAssertions (assertions), NSubstitute (mocking)
- Nullable reference types enabled
- File-scoped namespaces
- Primary constructors where appropriate

## Architecture Layers

Follow the Gathering reference project structure:

| Layer                   | Responsibility                                                | Dependencies             |
| ----------------------- | ------------------------------------------------------------- | ------------------------ |
| **Domain**              | Entities, value objects, domain events, repository interfaces | None (SharedKernel only) |
| **Application**         | Command/query handlers, validators, DTOs                      | Domain                   |
| **Infrastructure**      | Repository implementations, external services                 | Domain, Application      |
| **API**                 | Controllers, middleware, DI configuration                     | All layers               |
| **SharedKernel/Common** | Result, Error, base classes                                   | None                     |

## Code Conventions

### Result Pattern

All operations that can fail return `Result<T>` or `Result`:

```csharp
public static Result<Session> Create(Guid communityId, string title, string speaker, DateTimeOffset scheduledAt)
{
    if (string.IsNullOrWhiteSpace(title))
        return Result.Failure<Session>(SessionError.TitleRequired);

    var session = new Session { /* ... */ };
    session.Raise(new SessionCreatedDomainEvent(session.Id));
    return Result.Success(session);
}
```

### Entity Pattern

```csharp
public sealed partial class Session : AuditableEntity
{
    public Guid Id { get; private set; }
    // Private setters, factory methods for creation
    public static Result<Session> Create(/* params */) { /* ... */ }
}
```

### Command Handler Pattern

```csharp
public sealed class CreateSessionCommandHandler : ICommandHandler<CreateSessionCommand, Guid>
{
    // Constructor injection of focused interfaces
    public async Task<Result<Guid>> HandleAsync(CreateSessionCommand command, CancellationToken ct = default)
    {
        // 1. Validate → 2. Verify preconditions → 3. Execute domain logic → 4. Persist
    }
}
```

### Test Pattern

```csharp
public sealed class SessionTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var communityId = Guid.NewGuid();

        // Act
        var result = Session.Create(communityId, "Title", "Speaker", DateTimeOffset.UtcNow.AddDays(7));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Title.Should().Be("Title");
    }
}
```

## Project Structure

Each principle project uses:

```
SolidPrinciples.{Principle}/
├── Problem/             # Code that violates the principle
│   └── README.md        # What's wrong and why
├── Solution/            # Correct implementation
│   └── README.md        # What was fixed and why
└── README.md            # Principle overview
```

## Class Naming Conventions

| Type         | Pattern                          | Example                         |
| ------------ | -------------------------------- | ------------------------------- |
| Entity       | `{Name}`                         | `Session`, `Community`          |
| Interface    | `I{Name}`                        | `ISessionRepository`            |
| Command      | `{Action}{Entity}Command`        | `CreateSessionCommand`          |
| Handler      | `{Action}{Entity}CommandHandler` | `CreateSessionCommandHandler`   |
| Validator    | `{Command}Validator`             | `CreateSessionCommandValidator` |
| Error        | `{Entity}Error`                  | `SessionError`                  |
| Domain Event | `{Entity}{Action}DomainEvent`    | `SessionCreatedDomainEvent`     |
| Test         | `{ClassUnderTest}Tests`          | `SessionTests`                  |

## Procedure

1. **Identify the layer** where the component belongs (Domain, Application, Infrastructure)
2. **Check existing code** in the project for patterns to follow
3. **Implement** using the conventions above
4. **Add XML doc comments** on public APIs
5. **Create or update unit tests** in the corresponding test project
6. **Verify the build**: `dotnet build`
7. **Run tests**: `dotnet test`

## Reference

- Gathering backend: https://github.com/alexicaballero/gathering/tree/main/backend
- See [domain patterns reference](./references/domain-patterns.md) for detailed entity/value object templates
- See [testing patterns reference](./references/testing-patterns.md) for test organization and patterns
