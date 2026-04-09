# Problema — Violaciones de LSP

## ¿Qué está mal?

Estas clases violan el **Principio de Sustitución de Liskov** al crear jerarquías donde los subtipos NO PUEDEN ser sustituidos de forma segura por sus tipos base. El código compilará pero fallará en tiempo de ejecución con comportamiento inesperado o excepciones.

### MemberHierarchy (Infierno de verificación de tipo)

La clase base `Member` define votación y creación de propuestas, pero:

- **GuestMember** lanza `NotImplementedException` — obliga a los llamadores a verificar el tipo antes de llamar
- **InactiveMember** hace nada silenciosamente — rompe el contrato esperado sin advertencia

**Falla en tiempo de ejecución:**
```csharp
Member member = GetMember(); // Podría ser Guest o Inactive
member.Vote(proposalId, true); // ¡BOOM! Excepción o falla silenciosa
```

No puedes substituir `Member` con sus subtipos. El código debe verificar tipos:
```csharp
if (member is not GuestMember)  // Olor de código de verificación de tipo
    member.Vote(proposalId, true);
```

### SessionHierarchy (Contratos rotos)

La clase `SessionBase` define un contrato, pero las clases derivadas lo violan:

- **WorkshopSession** FORTALECE precondiciones: `Confirm()` requiere asistentes mínimos (clase base no)
- **PremiumSession** FORTALECE precondiciones: `RegisterAttendee()` requiere miembros verificados (clase base acepta cualquiera)
- **FreeSession** DEBILITA postcondiciones: `Cancel()` puede fallar con asistentes (clase base promete que siempre tiene éxito)

**Violación de contrato:**
```csharp
SessionBase session = GetSession(); // Podría ser Workshop, Premium o Free
session.Confirm(); // Podría fallar inesperadamente si es WorkshopSession
session.RegisterAttendee(memberId); // Podría fallar si Premium y miembro no verificado
session.Cancel(); // Podría fallar si Free y tiene asistentes
```

## 🔍 Señales de advertencia

| Olor de código | Dónde |
|---|---|
| `NotImplementedException` en métodos heredados | `GuestMember.Vote()` |
| Ignorar operaciones silenciosamente (no-op) | `InactiveMember.Vote()` |
| Verificación de tipo: `if (x is GuestMember)` | En cualquier parte usando jerarquía Member |
| Precondiciones fortalecidas en anulaciones | `WorkshopSession.Confirm()` |
| Postcondiciones debilitadas en anulaciones | `FreeSession.Cancel()` |
| Excepciones solo en subtipos | `PremiumSession.RegisterAttendee()` |

## 🧪 Dolor de pruebas

- No puedes escribir una prueba que funcione para **todos** los subtipos de Member — las pruebas fallan para GuestMember
- No puedes substituir `SessionBase` en pruebas — cada subtipo requiere configuración y aserciones diferentes
- Debes escribir caminos de prueba separados para cada tipo concreto en lugar de probar polimórficamente

## 📉 Impacto

When requirements change:

- **Add a new member action?** → All type-checking code must be updated
- **Reuse session logic?** → Cannot trust the base contract, must check runtime types
- **Write generic code?** → Impossible — every function must handle each subtype specially

The promise of inheritance and polymorphism is broken.

## Files

| File | Description |
|------|-------------|
| `MemberHierarchy.cs` | Member types where subtypes throw exceptions or silently fail |
| `SessionHierarchy.cs` | Session types that strengthen preconditions or weaken postconditions |


