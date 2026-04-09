# DIP Content Guide

## Dependency Inversion Principle

> High-level modules should not depend on low-level modules. Both should depend on abstractions.

## Learning Objectives

After this module, students should be able to:

1. Identify direct dependencies on concrete implementations
2. Recognize `new` keywords in business logic as DIP violations
3. Introduce abstractions between layers
4. Configure dependency injection for loose coupling

## Violation Scenarios to Implement

### Scenario 1: Direct Infrastructure Dependencies

A command handler that directly instantiates its dependencies:

```csharp
// Problem/CreateSessionHandler.cs
public class CreateSessionHandler
{
    public async Task<Guid> Handle(CreateSessionCommand command)
    {
        // Direct dependency on concrete SQL implementation
        var repository = new SqlSessionRepository("Server=localhost;Database=Gathering;...");

        // Direct dependency on concrete email service
        var emailService = new SmtpEmailService("smtp.company.com", 587);

        // Direct dependency on concrete logger
        var logger = new FileLogger("C:\\logs\\sessions.log");

        // Business logic tightly coupled to infrastructure
        var session = new Session { Title = command.Title };
        await repository.SaveAsync(session);
        await emailService.SendAsync("admin@company.com", "New session created");
        logger.Log($"Session {session.Id} created");

        return session.Id;
    }
}
```

### Scenario 2: Coupled Reporting

A community report generator that directly uses SQL and file system.

## Solution Scenarios to Implement

### Scenario 1: Abstraction-Based Handler

```csharp
// Solution/CreateSessionCommandHandler.cs
public sealed class CreateSessionCommandHandler
{
    private readonly ISessionRepository _sessionRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CreateSessionCommandHandler> _logger;

    // Dependencies injected through constructor
    public CreateSessionCommandHandler(
        ISessionRepository sessionRepository,
        INotificationService notificationService,
        ILogger<CreateSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    // High-level policy depends only on abstractions
    public async Task<Result<Guid>> HandleAsync(CreateSessionCommand command)
    {
        var result = Session.Create(command.CommunityId, command.Title, command.Speaker, command.ScheduledAt);
        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        _sessionRepository.Add(result.Value);
        _logger.LogInformation("Session {SessionId} created", result.Value.Id);

        return Result.Success(result.Value.Id);
    }
}
```

### Scenario 2: DI Registration Example

Show how abstractions are wired to implementations at composition root.

## Key Testing Points

- Violation: Tests require real database, SMTP server, file system
- Solution: All dependencies are substitutable mocks
- Show DI container registration as the "composition root"

## Article Reference

https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/dependency-inversion
