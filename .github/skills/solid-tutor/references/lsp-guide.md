# LSP Content Guide

## Liskov Substitution Principle

> Objects of a derived class should be able to replace objects of a base class without breaking the application.

## Learning Objectives

After this module, students should be able to:
1. Identify inheritance hierarchies that violate substitutability
2. Recognize when a subclass changes or restricts base class behavior
3. Design proper inheritance using contracts
4. Prefer composition when inheritance doesn't fit

## Violation Scenarios to Implement

### Scenario 1: Session Type Hierarchy

A `RecurringSession` that inherits from `Session` but violates its contract:

```csharp
// Problem/RecurringSession.cs
public class RecurringSession : Session
{
    public override Result UpdateStatus(SessionStatus newStatus)
    {
        // Throws NotImplementedException for some statuses
        // Base class Session allows all valid transitions
        if (newStatus == SessionStatus.Canceled)
            throw new NotSupportedException("Recurring sessions cannot be canceled individually");

        return base.UpdateStatus(newStatus);
    }
}
```

### Scenario 2: ReadOnlyCommunity

A `ReadOnlyCommunity` extending `Community` that throws on mutation methods.

## Solution Scenarios to Implement

### Scenario 1: Proper Session Abstractions

```csharp
// Solution/ISchedulable.cs
public interface ISchedulable
{
    Result UpdateStatus(SessionStatus newStatus);
    IReadOnlyCollection<SessionStatus> AllowedTransitions { get; }
}

// Each type declares its own valid transitions without violating contracts
public sealed class SingleSession : ISchedulable { }
public sealed class RecurringSession : ISchedulable { }
```

### Scenario 2: Composition-Based Community Access

Using separate interfaces for read and write operations.

## Key Testing Points

- Violation: Tests passing base type fail when substituting derived type
- Solution: Any implementation of the interface passes the same contract tests
- Demonstrate contract testing: one test suite that all implementations must pass

## Article Reference

https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/liskov-substitution
