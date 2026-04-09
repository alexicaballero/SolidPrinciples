# OCP Content Guide

## Open/Closed Principle

> A class should be open for extension but closed for modification.

## Learning Objectives

After this module, students should be able to:

1. Identify code that must be modified to add new behavior
2. Recognize if-else/switch chains as OCP violations
3. Apply Strategy, Template Method, and plugin patterns
4. Extend behavior without touching existing code

## Violation Scenarios to Implement

### Scenario 1: Session Export with Switch

A session exporter that uses a switch statement to select format:

```csharp
// Problem/SessionExporter.cs
public class SessionExporter
{
    public string Export(Session session, string format)
    {
        switch (format)
        {
            case "json": return JsonSerializer.Serialize(session);
            case "csv": return $"{session.Title},{session.Speaker},{session.ScheduledAt}";
            case "xml": return $"<session><title>{session.Title}</title>...</session>";
            default: throw new ArgumentException($"Unsupported format: {format}");
        }
        // Adding PDF requires modifying this class
    }
}
```

### Scenario 2: Notification Dispatcher

A notification service with if-else chain checking notification type.

## Solution Scenarios to Implement

### Scenario 1: Strategy Pattern Export

```csharp
// Solution/ISessionExportStrategy.cs
public interface ISessionExportStrategy
{
    string Format { get; }
    string Export(Session session);
}

// New formats added by creating new classes, not modifying existing
public sealed class JsonSessionExporter : ISessionExportStrategy { }
public sealed class CsvSessionExporter : ISessionExportStrategy { }
```

### Scenario 2: Plugin-Based Notifications

Using interfaces and DI to dispatch notifications without modification.

## Key Testing Points

- Violation: Adding new format requires changing tests for existing formats
- Solution: Each exporter testable independently; new formats = new test classes

## Article Reference

https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/open-closed
