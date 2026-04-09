# Problema — Violaciones de LSP

## ¿Qué está mal?

Estas clases violan el **Principio de Sustitución de Liskov** al crear jerarquías donde los subtipos NO PUEDEN ser sustituidos de forma segura por sus tipos base. El código compilará pero fallará en tiempo de ejecución con comportamiento inesperado o excepciones.

### 1. EntityHierarchy (Precondiciones fortalecidas e invariantes rotos)

La jerarquía de `Entity` en este ejemplo viola LSP de dos formas fundamentales:

- **RestrictedEntity** FORTALECE precondiciones: el método `Raise()` que debe aceptar cualquier `IDomainEvent` es sobrescrito para aceptar solo `AuditEvent`
- **BrokenAuditableEntity** ROMPE invariantes: `CreatedAt` puede ser `null`, violando la garantía de que una entidad auditable siempre tiene timestamp de creación

**Falla en tiempo de ejecución:**

```csharp
Entity entity = new RestrictedSession();
entity.Raise(new SessionCreatedEvent(id)); // ¡BOOM! Solo acepta AuditEvent
```

### 2. ShapeHierarchy (El clásico Rectángulo/Cuadrado)

La clase `Square` hereda de `Rectangle`, pero rompe el contrato de independencia de propiedades:

- **Square** tiene efectos secundarios ocultos: asignar `Width` también modifica `Height`, y viceversa
- El contrato implícito de `Rectangle` es que `Width` y `Height` son independientes
- No puedes tratar un `Square` como un `Rectangle` sin obtener resultados incorrectos

**Violación de contrato:**

```csharp
Rectangle rect = new Square(5);
rect.Width = 10;
rect.Height = 5;
int area = rect.CalculateArea(); // Esperado: 50, Real: 25 (5×5)
```

### 3. MemberHierarchy (Infierno de verificación de tipo)

La clase base `Member` define votación y creación de propuestas, pero:

- **GuestMember** lanza `NotImplementedException` — obliga a los llamadores a verificar el tipo antes de llamar
- **InactiveMember** hace nada silenciosamente — rompe el contrato esperado sin advertencia

**Falla en tiempo de ejecución:**

```csharp
Member member = GetMember(); // Podría ser Guest o Inactive
member.Vote(proposalId, true); // ¡BOOM! Excepción o falla silenciosa
```

No puedes sustituir `Member` con sus subtipos. El código debe verificar tipos:

```csharp
if (member is not GuestMember)  // Olor de código de verificación de tipo
    member.Vote(proposalId, true);
```

### 4. SessionHierarchy (Contratos rotos)

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

| Olor de código                                 | Dónde                                                           |
| ---------------------------------------------- | --------------------------------------------------------------- |
| Precondiciones fortalecidas en subtipos        | `RestrictedEntity.Raise()`, `PremiumSession.RegisterAttendee()` |
| Invariantes rotos                              | `BrokenAuditableEntity.CreatedAt` puede ser null                |
| Efectos secundarios ocultos                    | `Square.Width` setter modifica `Height`                         |
| `NotImplementedException` en métodos heredados | `GuestMember.Vote()`                                            |
| Ignorar operaciones silenciosamente (no-op)    | `InactiveMember.Vote()`                                         |
| Verificación de tipo: `if (x is GuestMember)`  | En cualquier parte usando jerarquía Member                      |
| Postcondiciones debilitadas en anulaciones     | `FreeSession.Cancel()`                                          |
| Excepciones solo en subtipos                   | `PremiumSession.RegisterAttendee()`                             |

## 🧪 Dolor de pruebas

- No puedes escribir una prueba que funcione para **todos** los subtipos de `Entity` — fallan con `RestrictedEntity`
- No puedes escribir cálculos genéricos que funcionen para todos los `Rectangle` — los resultados son incorrectos con `Square`
- No puedes escribir una prueba que funcione para **todos** los subtipos de `Member` — las pruebas fallan para `GuestMember`
- No puedes sustituir `SessionBase` en pruebas — cada subtipo requiere configuración y aserciones diferentes
- Debes escribir caminos de prueba separados para cada tipo concreto en lugar de probar polimórficamente

## 📉 Impacto

Cuando los requisitos cambian:

- **¿Agregar un nuevo tipo de evento de dominio?** → RestrictedEntity rechazará eventos válidos
- **¿Usar Square en código que espera Rectangle?** → Cálculos incorrectos garantizados
- **¿Agregar una nueva acción de miembro?** → Todo el código de verificación de tipo debe actualizarse
- **¿Reutilizar lógica de sesión?** → No puedes confiar en el contrato base, debes verificar tipos en tiempo de ejecución
- **¿Escribir código genérico?** → Imposible — cada función debe manejar cada subtipo especialmente

La promesa de la herencia y el polimorfismo se rompe.

## Archivos por caso de uso

### EntityHierarchy

| Archivo                    | Descripción                                                |
| -------------------------- | ---------------------------------------------------------- |
| `Entity.cs`                | Clase base con infraestructura de eventos de dominio       |
| `RestrictedEntity.cs`      | VIOLACIÓN: Fortalece precondiciones en `Raise()`           |
| `BrokenAuditableEntity.cs` | VIOLACIÓN: Rompe invariantes con `CreatedAt` nullable      |
| `Consumers.cs`             | Código de consumo que falla con los subtipos problemáticos |
| `DomainEvents.cs`          | Eventos de dominio de ejemplo                              |

### ShapeHierarchy

| Archivo                  | Descripción                                                    |
| ------------------------ | -------------------------------------------------------------- |
| `Rectangle.cs`           | Clase base con Width y Height independientes                   |
| `Square.cs`              | VIOLACIÓN: Hereda de Rectangle con efectos secundarios ocultos |
| `GeometryService.cs`     | Servicio que falla con Square                                  |
| `ShapeBatchProcessor.cs` | Procesamiento por lotes que produce resultados incorrectos     |

### MemberHierarchy

| Archivo              | Descripción                                                                 |
| -------------------- | --------------------------------------------------------------------------- |
| `MemberHierarchy.cs` | Jerarquía Member donde subtipos lanzan excepciones o fallan silenciosamente |

### SessionHierarchy

| Archivo               | Descripción                                                               |
| --------------------- | ------------------------------------------------------------------------- |
| `SessionHierarchy.cs` | Jerarquía Session que fortalece precondiciones y debilita postcondiciones |
