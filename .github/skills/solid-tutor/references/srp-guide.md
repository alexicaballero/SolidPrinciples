# SRP Content Guide

## Single Responsibility Principle

> A class should have one, and only one, reason to change.

## Learning Objectives

After this module, students should be able to:
1. Identify classes with multiple responsibilities
2. Recognize warning signs (God classes, mixed concerns)
3. Extract focused classes with single responsibilities
4. Test each responsibility independently

## Violation Scenarios to Implement

### Scenario 1: SessionManager God Class

A single class that handles:
- Session validation
- Session persistence (direct database access)
- Email notification to community admin
- Logging session operations

```csharp
// Problem/SessionManager.cs
public class SessionManager
{
    public void CreateSession(string title, string speaker, DateTime date, Guid communityId)
    {
        // Validates input (responsibility 1)
        // Queries database for community (responsibility 2)
        // Creates session record (responsibility 3)
        // Sends email notification (responsibility 4)
        // Logs the operation (responsibility 5)
    }
}
```

### Scenario 2: CommunityReport (from article)

A report service that fetches data, formats it, and sends it — mirroring the article's `ReportService` example.

## Solution Scenarios to Implement

### Scenario 1: Separated Responsibilities

- `Session` entity with factory method and domain events
- `CreateSessionCommandHandler` for orchestration
- `ISessionRepository` for persistence
- `INotificationService` for notifications

### Scenario 2: Separated Report Components

- `ISalesDataProvider` for data access
- `IReportFormatter` for formatting
- `IReportSender` for email/delivery

## Key Testing Points

- Violation: Tests require full infrastructure setup
- Solution: Each class testable in isolation
- Show the difference in test complexity and speed

## Article Reference

https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/single-responsibility
