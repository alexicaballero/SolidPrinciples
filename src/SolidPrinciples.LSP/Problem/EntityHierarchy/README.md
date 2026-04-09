# Caso de Uso: Jerarquía de Entidades (Entity Hierarchy)

> **Learning Objective**: Understand how proper inheritance hierarchies respect LSP by adding behavior without modifying base contracts, and how violations break polymorphic code.

## 📚 Reference Article

This case study implements the example from:
**[Principio de Sustitución de Liskov - Caso de Estudio: Jerarquía de Entidades en Gathering](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution#caso-de-estudio-jerarqu%C3%ADa-de-entidades-en-gathering)**

Gathering's entity hierarchy demonstrates LSP at its simplest and most fundamental level.

## 🎯 The Scenario

You're building a domain model with a base `Entity` class that provides domain event infrastructure. Entities can raise events (like `SessionCreatedEvent`) that are later dispatched by event handlers.

**The contract**: Any entity can raise any `IDomainEvent` without restrictions.

## Problem: Violating LSP

### Violation 1: Strengthening Preconditions

```csharp
public abstract class Entity
{
    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent); // Accepts ANY IDomainEvent
    }
}

// VIOLATION: Subtype requires MORE than base type
public abstract class RestrictedEntity : Entity
{
    protected new void Raise(IDomainEvent domainEvent)
    {
        // Strengthened precondition: now requires AuditEvent specifically
        if (domainEvent is not AuditEvent)
            throw new ArgumentException("Only AuditEvent allowed");

        base.Raise(domainEvent);
    }
}
```

**Why this violates LSP**:

- **Base contract**: `Raise()` accepts **any** `IDomainEvent`
- **Subtype contract**: `Raise()` accepts **only** `AuditEvent`
- **Precondition strengthened**: Subtype requires MORE than base type
- **Substitution broken**: Code expecting `Entity` cannot safely use `RestrictedEntity`

**Impact**:

```csharp
// Works with Entity
Entity entity = new NormalSession();
entity.Raise(new SessionCreatedEvent(id)); // ✓ OK

// Breaks with RestrictedEntity
Entity entity = new RestrictedSession();
entity.Raise(new SessionCreatedEvent(id)); // ✗ Throws ArgumentException!
```

### Violation 2: Breaking Invariants

```csharp
// VIOLATION: Nullable timestamp breaks the auditable invariant
public abstract class BrokenAuditableEntity : Entity
{
    public DateTimeOffset? CreatedAt { get; set; } // Can be null!
}
```

**Why this violates LSP**:

- **Expected invariant**: Auditable entities ALWAYS have a creation timestamp
- **Broken invariant**: `CreatedAt` can be `null`
- **Client code breaks**: Queries expecting timestamps fail

**Impact**:

```csharp
// Repository code MUST defend against null
var recent = entities
    .Where(e => e.CreatedAt.HasValue && e.CreatedAt.Value > since) // Defensive!
    .ToList();
```

## ✅ Solution: Respecting LSP

### The Correct Hierarchy

```
Entity (base with domain events)
  ↓
AuditableEntity (adds timestamps, preserves base contract)
  ↓
Session / Community (sealed concrete entities)
```

### How It Works

#### 1. Entity establishes the base contract

```csharp
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent); // No restrictions
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

**Contract established**:

- ✓ Any entity can raise any `IDomainEvent`
- ✓ Events are collected and can be cleared
- ✓ No subtype should modify this behavior

#### 2. AuditableEntity EXTENDS without changing

```csharp
public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; private set; } // Non-nullable!
    public DateTimeOffset? UpdatedAt { get; private set; }

    protected AuditableEntity()
    {
        CreatedAt = DateTimeOffset.UtcNow; // Always initialized
    }

    protected void MarkAsUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
```

**Why this respects LSP**:

- ✓ Does NOT override `Raise()` - inherits as-is
- ✓ ADDS properties (`CreatedAt`, `UpdatedAt`) without changing base behavior
- ✓ Guarantees `CreatedAt` is ALWAYS set (non-nullable, constructor-initialized)
- ✓ Can substitute for `Entity` anywhere without breaking code

#### 3. Session and Community are sealed concrete types

```csharp
public sealed class Session : AuditableEntity
{
    public static Result<Session> Create(string title, string speaker, DateTimeOffset scheduledAt)
    {
        // Validations...

        var session = new Session { /* props */ };

        // ✓ Uses inherited Raise() exactly as defined
        session.Raise(new SessionCreatedDomainEvent(session.Id));

        return Result.Success(session);
    }

    public Result Update(string title, string speaker)
    {
        // Validations...

        Title = title;
        Speaker = speaker;

        // ✓ Uses BOTH Entity.Raise() and AuditableEntity.MarkAsUpdated()
        MarkAsUpdated();
        Raise(new SessionUpdatedDomainEvent(Id));

        return Result.Success();
    }
}
```

**Why `sealed` is important**:

- Prevents further subclassing that could violate contracts
- `Session` and `Community` have specific behavior (state machines, validation)
- Extending them could easily introduce LSP violations

## 🎯 Benefits of LSP Compliance

### 1. **Generic Event Dispatcher Works**

```csharp
public class EventDispatcher
{
    public void DispatchEvents(Entity entity) // Accepts ANY Entity
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            // Process event...
        }
        entity.ClearDomainEvents();
    }
}

// ✓ Works with Session
dispatcher.DispatchEvents(session);

// ✓ Works with Community
dispatcher.DispatchEvents(community);

// ✓ Works with ANY future entity
dispatcher.DispatchEvents(newEntityType);
```

**No type checking. No special cases. Just polymorphism.**

### 2. **Generic Repository Works**

```csharp
public class AuditableRepository<T> where T : AuditableEntity
{
    public IReadOnlyList<T> GetRecentEntities(DateTimeOffset since)
    {
        return _entities
            .Where(e => e.CreatedAt > since) // No null check needed!
            .ToList();
    }
}

// ✓ Works with Session
var repository = new AuditableRepository<Session>();

// ✓ Works with Community
var repository = new AuditableRepository<Community>();
```

**Trust the contract. No defensive coding.**

### 3. **Safe Extension**

Adding a new entity type (e.g., `Resource`) is safe:

```csharp
public sealed class Resource : AuditableEntity
{
    // Automatically gets domain events (from Entity)
    // Automatically gets audit timestamps (from AuditableEntity)
    // Works with EventDispatcher, AuditableRepository, etc.
}
```

**Extend once, benefit everywhere.**

## 🔍 Key Takeaways

| Rule                                | Violation Example                     | Correct Approach                       |
| ----------------------------------- | ------------------------------------- | -------------------------------------- |
| **Preconditions cannot strengthen** | Requiring `AuditEvent` only           | Accept any `IDomainEvent`              |
| **Postconditions cannot weaken**    | Returning `null` when timestamp due   | Guarantee non-null with initialization |
| **Invariants must be preserved**    | Nullable `CreatedAt` in auditable     | Non-nullable, constructor-initialized  |
| **Inheritance = "is-a" relation**   | `RestrictedEntity` is NOT an `Entity` | `Session` IS an `AuditableEntity`      |
| **Composition over inheritance**    | Don't subclass to change behavior     | Subclass to EXTEND, compose to VARY    |

## 📖 Comparison with Gathering Reference

This implementation directly mirrors the **Gathering** reference architecture:

| Gathering                                                                                                           | This Implementation      |
| ------------------------------------------------------------------------------------------------------------------- | ------------------------ |
| [Entity.cs](https://github.com/alexicaballero/gathering/blob/main/backend/SharedKernel/Entity.cs)                   | `Entity` class           |
| [AuditableEntity.cs](https://github.com/alexicaballero/gathering/blob/main/backend/SharedKernel/AuditableEntity.cs) | `AuditableEntity` class  |
| [Session.cs](https://github.com/alexicaballero/gathering/blob/main/backend/Domain/Sessions/Session.cs)              | `Session` sealed class   |
| [Community.cs](https://github.com/alexicaballero/gathering/blob/main/backend/Domain/Communities/Community.cs)       | `Community` sealed class |

**Study the Gathering codebase to see this pattern in production.**

## 🧪 How to Verify

Run the tests:

```bash
dotnet test --filter "FullyQualifiedName~EntityHierarchy"
```

The tests demonstrate:

- ✗ Problem: `RestrictedEntity` throws exceptions, breaking polymorphism
- ✓ Solution: `Session` and `Community` work interchangeably with `Entity`-based code
