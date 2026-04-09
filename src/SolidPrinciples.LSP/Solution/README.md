# Solución — LSP aplicado

## Qué se corrigió

Ambas jerarquías fueron refactorizadas para asegurar **substituibilidad**: puedes usar cualquier instancia sin verificación de tipo, excepciones o cambios de comportamiento inesperados.

### Member — Composición sobre herencia

**Antes:** Jerarquía de herencia donde `GuestMember` lanzaba excepciones e `InactiveMember` fallaba silenciosamente.  
**Después:** Una sola clase `Member` con **diseño basado en capacidades**.

- Todos los Members tienen la misma interfaz
- Las capacidades son consultables: `member.Capabilities.CanVote`
- Las operaciones devuelven `Result` con razones de falla claras — sin excepciones, sin fallas silenciosas
- Métodos de fábrica: `Member.CreateFull()`, `Member.CreateGuest()`, `Member.CreateInactive()`

**Beneficio:**
```csharp
// Puedes substituir cualquier Member sin verificación de tipo
Member member = GetMember(); // Full, Guest o Inactive — no importa
var result = member.Vote(proposalId, true);
if (result.IsFailure)
    HandleError(result.Error); // Razón de falla clara
```

### Session — Patrón de estrategia con políticas

**Antes:** Jerarquía de herencia donde `WorkshopSession` fortalecía precondiciones y `FreeSession` debilitaba postcondiciones.  
**Después:** Una sola clase `Session` con **objetos políticos inyectados**.

- Una sola clase `Session` — sin subtipos
- Las políticas implementan `ISessionPolicy` para hacer cumplir reglas diferentes
- Todas las sesiones respetan el mismo contrato: las operaciones devuelven `Result`
- Métodos de fábrica inyectan políticas apropiadas: `Session.CreateWorkshop()`, `Session.CreatePremium()`, etc.

**Beneficio:**
```csharp
// Puedes substituir cualquier Session sin sorpresas en tiempo de ejecución
Session session = GetSession(); // Standard, Workshop, Premium, Free — todos funcionan igual
var result = session.RegisterAttendee(memberId);
// Sin excepciones inesperadas, sin verificación de tipo necesaria
```

## Patrones aplicados

| Patrón | Dónde | Por qué |
|--------|-------|--------|
| **Composición sobre herencia** | `Member` con `MemberCapabilities` | Evita jerarquías de herencia frágiles |
| **Patrón de estrategia** | `Session` con `ISessionPolicy` | Variación de política sin violaciones de subtipo |
| **Método de fábrica** | `Member.CreateGuest()`, `Session.CreateWorkshop()` | Encapsula configuración de política/capacidad |
| **Patrón Result** | Todas las operaciones devuelven `Result` | Sin excepciones, éxito/falla explícita |

## Beneficios obtenidos

- **Substituibilidad**: Cualquier `Member` puede usarse en cualquier parte — sin verificación de tipo, sin fallas en tiempo de ejecución
- **Contratos explícitos**: Las capacidades y políticas son consultables y testeables
- **Sin excepciones**: Las operaciones devuelven `Result` con códigos de error claros
- **Extensibilidad**: Agrega nuevas capacidades o políticas sin modificar código existente
- **Testabilidad**: Simula `ISessionPolicy` para probar la lógica de Session de forma aislada

## 🧪 Test Highlights

- Tests can work polymorphically with `Member` — all instances behave consistently
- Session tests inject different policies without duplicating Session test logic
- No need for try-catch or type-checking in tests

## Files

| File | Description |
|------|-------------|
| `Member.cs` | Single Member class with capability-based design (composition) |
| `Session.cs` | Single Session class with policy-based validation (strategy) |

## 📖 LSP in Practice

**Liskov Substitution Principle states:**

> "Objects of a superclass should be replaceable with objects of its subclasses without affecting correctness."

Our solution achieves this by:

1. **Avoiding inheritance where behavior differs** — use composition instead
2. **Honoring contracts** — operations return `Result`, never throw unexpected exceptions
3. **Queryable behavior** — capabilities and policies are explicit, not hidden in subtype logic
4. **Consistent interfaces** — all Members/Sessions respond to the same methods with predictable contracts


