# Caso de Uso: Jerarquía de Entidades (Entity Hierarchy) — Solución

> **Objetivo de aprendizaje**: Entender cómo las jerarquías de herencia correctamente diseñadas respetan LSP al extender comportamiento sin modificar contratos base.

## 📚 Artículo de referencia

Esta solución implementa el diseño correcto explicado en:
**[Principio de Sustitución de Liskov - Caso de Estudio: Jerarquía de Entidades en Gathering](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution#caso-de-estudio-jerarqu%C3%ADa-de-entidades-en-gathering)**

## 🎯 La solución: Herencia que respeta LSP

### La jerarquía correcta

```
Entity (base con eventos de dominio)
  ↓
AuditableEntity (añade timestamps, preserva contrato base)
  ↓
Session / Community (entidades concretas sealed)
```

### Cómo funciona

#### 1. Entity establece el contrato base

```csharp
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent); // Sin restricciones
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

**Contrato establecido:**

- ✓ Cualquier entidad puede levantar cualquier `IDomainEvent`
- ✓ Los eventos son recopilados y pueden limpiarse
- ✓ Ningún subtipo debe modificar este comportamiento

#### 2. AuditableEntity EXTIENDE sin cambiar

```csharp
public abstract class AuditableEntity : Entity
{
    public DateTimeOffset CreatedAt { get; private set; } // No anulable!
    public DateTimeOffset? UpdatedAt { get; private set; }

    protected AuditableEntity()
    {
        CreatedAt = DateTimeOffset.UtcNow; // Siempre inicializado
    }

    protected void MarkAsUpdated()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
```

**Por qué esto respeta LSP:**

- ✓ NO sobrescribe `Raise()` - lo hereda tal cual
- ✓ AGREGA propiedades (`CreatedAt`, `UpdatedAt`) sin cambiar el comportamiento base
- ✓ Garantiza que `CreatedAt` esté SIEMPRE establecido (no anulable, inicializado en constructor)
- ✓ Puede sustituirse por `Entity` en cualquier lugar sin romper el código

#### 3. Session y Community son tipos concretos sealed

```csharp
public sealed class Session : AuditableEntity
{
    public static Result<Session> Create(string title, string speaker, DateTimeOffset scheduledAt)
    {
        // Validaciones...

        var session = new Session { /* props */ };

        // ✓ Usa Raise() heredado exactamente como se definió
        session.Raise(new SessionCreatedDomainEvent(session.Id));

        return Result.Success(session);
    }

    public Result Update(string title, string speaker)
    {
        // Validaciones...

        Title = title;
        Speaker = speaker;

        // ✓ Usa AMBOS: Entity.Raise() y AuditableEntity.MarkAsUpdated()
        MarkAsUpdated();
        Raise(new SessionUpdatedDomainEvent(Id));

        return Result.Success();
    }
}
```

**Por qué `sealed` es importante:**

- Previene la creación de subclases adicionales que podrían violar contratos
- `Session` y `Community` tienen comportamiento específico (máquinas de estado, validación)
- Extenderlas podría fácilmente introducir violaciones de LSP

## 🎯 Beneficios de cumplir con LSP

### 1. Despachador de eventos genérico funciona

```csharp
public sealed class DomainEventDispatcher
{
    public async Task DispatchEvents(Entity entity) // Acepta cualquier Entity
    {
        foreach (var domainEvent in entity.DomainEvents)
        {
            await PublishAsync(domainEvent); // ✓ Funciona con todos los subtipos
        }

        entity.ClearDomainEvents();
    }
}
```

**Polimorfismo en acción:** El despachador funciona con `Session`, `Community`, o cualquier futura entidad sin modificación.

### 2. Los repositorios pueden manejar eventos genéricamente

```csharp
public abstract class Repository<T> where T : Entity
{
    protected async Task SaveAsync(T entity)
    {
        await _dbContext.SaveChangesAsync();

        // ✓ Funciona para cualquier entidad - sin verificación de tipo
        await _eventDispatcher.DispatchEvents(entity);
    }
}
```

### 3. Las consultas confían en los invariantes

```csharp
public async Task<List<Session>> GetRecentSessions(DateTimeOffset since)
{
    // ✓ Podemos confiar en que CreatedAt NUNCA es null
    return await _dbContext.Sessions
        .Where(s => s.CreatedAt > since) // Sin defensivas HasValue
        .ToListAsync();
}
```

**Sin AuditableEntity correcto:** Necesitarías `s.CreatedAt.HasValue && s.CreatedAt.Value > since` por todas partes.

### 4. Las pruebas funcionan polimórficamente

```csharp
[Fact]
public void AllEntities_CanRaiseAndClearEvents()
{
    // Arrange: Cualquier entidad funciona
    Entity[] entities = [
        Session.Create("Title", "Speaker", futureDate).Value,
        Community.Create("Name", "Description", adminId).Value
    ];

    // Act & Assert: El mismo código funciona para todas
    foreach (var entity in entities)
    {
        entity.Raise(new TestEvent());
        Assert.Single(entity.DomainEvents);

        entity.ClearDomainEvents();
        Assert.Empty(entity.DomainEvents);
    }
}
```

## ✅ Cómo esto previene las violaciones del problema

| Violación en Problem/                               | Cómo se previene en Solution/                                                        |
| --------------------------------------------------- | ------------------------------------------------------------------------------------ |
| `RestrictedEntity.Raise()` fortalece precondiciones | `Entity.Raise()` nunca se sobrescribe - todos los subtipos lo heredan sin cambios    |
| `BrokenAuditableEntity.CreatedAt` puede ser null    | `AuditableEntity.CreatedAt` es no anulable e inicializado en el constructor          |
| Los consumidores deben verificar tipos              | Todos los subtipos respetan el contrato de `Entity` - el código polimórfico funciona |
| Defensivas en consultas                             | Los invariantes están garantizados - no se necesitan verificaciones                  |

## 📖 Reglas de LSP aplicadas

| Regla                                         | Cumplimiento                                                                        |
| --------------------------------------------- | ----------------------------------------------------------------------------------- |
| **Las precondiciones no pueden fortalecerse** | ✓ `AuditableEntity` no cambia el método `Raise()` - acepta cualquier `IDomainEvent` |
| **Las postcondiciones no pueden debilitarse** | ✓ Todos los métodos heredados funcionan exactamente como se espera                  |
| **Los invariantes deben preservarse**         | ✓ `CreatedAt` está garantizado como no-null en construcción                         |

## 🧪 Destacados de las pruebas

```csharp
// Las pruebas pueden funcionar con Entity sin conocer los tipos concretos
public void TestEntityBehavior(Entity entity)
{
    entity.Raise(new TestEvent());
    Assert.NotEmpty(entity.DomainEvents);
}

// Las entidades auditables siempre tienen timestamps válidos
public void TestAuditableEntity(AuditableEntity auditable)
{
    Assert.NotEqual(default, auditable.CreatedAt);
    // No se necesita verificación de null - garantizado por tipo
}
```

## 🚀 Extensibilidad

¿Quieres agregar una nueva entidad como `Proposal` o `Attendee`?

```csharp
public sealed class Proposal : AuditableEntity
{
    // ✓ Hereda Raise() sin restricciones
    // ✓ Obtiene CreatedAt/UpdatedAt automáticamente
    // ✓ Puede sustituirse por Entity o AuditableEntity en cualquier lugar
}
```

**Sin cambios en:**

- Despachador de eventos
- Repositorios base
- Consultas genéricas
- Código de pruebas

## Archivos

| Archivo              | Descripción                                                                  |
| -------------------- | ---------------------------------------------------------------------------- |
| `Entity.cs`          | Clase base que define el contrato de eventos de dominio                      |
| `AuditableEntity.cs` | Extiende Entity agregando auditoría sin modificar el comportamiento heredado |
| `Session.cs`         | Entidad concreta sealed que implementa sesiones de CoP                       |
| `Community.cs`       | Entidad concreta sealed que implementa comunidades de práctica               |
| `Consumers.cs`       | Despachador de eventos y repositorios que funcionan polimórficamente         |
| `DomainEvents.cs`    | Implementaciones de eventos de dominio de ejemplo                            |

## 💡 Conclusiones clave

1. **La herencia es correcta cuando extiende sin modificar** — `AuditableEntity` agrega propiedades pero no cambia métodos
2. **Los invariantes deben ser inquebrantables** — `CreatedAt` no anulable + inicialización en constructor
3. **El `sealed` previene violaciones futuras** — las entidades concretas no pueden ser extendidas incorrectamente
4. **El código polimórfico debe funcionar sin verificación de tipo** — los despachadores, repositorios y consultas funcionan con cualquier `Entity`

Esta es la herencia como debería ser: **extensión predecible y segura** que respeta todos los contratos.
