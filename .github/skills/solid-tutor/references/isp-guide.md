# ISP Content Guide

## Interface Segregation Principle

> Clients should never be forced to depend on interfaces they do not use.

## Learning Objectives

After this module, students should be able to:

1. Identify fat interfaces that force unnecessary dependencies
2. Recognize classes that implement interfaces with empty/throwing methods
3. Split interfaces into focused, role-based contracts
4. Design interfaces from the client's perspective

## Violation Scenarios to Implement

### Scenario 1: Fat Repository Interface

A single `ICommunityRepository` with every possible operation:

```csharp
// Problem/ICommunityRepository.cs
public interface ICommunityRepository
{
    Task<Community?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Community>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Community>> SearchAsync(string query, CancellationToken ct = default);
    void Add(Community community);
    void Update(Community community);
    void Delete(Community community);
    Task<int> CountAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Session>> GetSessionsAsync(Guid communityId, CancellationToken ct = default);
    Task BulkInsertAsync(IEnumerable<Community> communities, CancellationToken ct = default);
    Task ExportAsync(Stream output, CancellationToken ct = default);
    // Clients that only need GetByIdAsync must depend on ALL of these
}
```

### Scenario 2: Fat Member Service Interface

An `IMemberService` that combines profile management, authentication, and notifications.

## Solution Scenarios to Implement

### Scenario 1: Role-Based Repository Interfaces

```csharp
// Solution/ — Focused interfaces
public interface ICommunityReader
{
    Task<Community?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Community>> GetAllAsync(CancellationToken ct = default);
}

public interface ICommunityWriter
{
    void Add(Community community);
    void Update(Community community);
}

public interface ICommunitySearch
{
    Task<IReadOnlyList<Community>> SearchAsync(string query, CancellationToken ct = default);
}
```

### Scenario 2: Segregated Member Interfaces

Split into `IMemberProfile`, `IMemberAuth`, `IMemberNotifications`.

## Key Testing Points

- Violation: Mocks must implement many unused methods
- Solution: Mocks are minimal and focused
- Show how test setup becomes simpler

## Article Reference

https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/interface-segregation
