# Solución — LSP aplicado

## Qué se corrigió

Las cuatro jerarquías fueron refactorizadas para asegurar **substituibilidad**: puedes usar cualquier instancia sin verificación de tipo, excepciones o cambios de comportamiento inesperados.

### 1. EntityHierarchy — Herencia correcta que extiende sin modificar

**Antes:** `RestrictedEntity` fortalecía precondiciones limitando el tipo de eventos aceptados; `BrokenAuditableEntity` rompía invariantes con timestamps nullable.

**Después:** Jerarquía correcta `Entity` → `AuditableEntity` → `Session`/`Community` que respeta LSP.

- `Entity` define el contrato base: cualquier entidad puede levantar cualquier `IDomainEvent`
- `AuditableEntity` **extiende sin modificar**: agrega `CreatedAt` y `UpdatedAt` no-nullable sin alterar `Raise()`
- `Session` y `Community` son `sealed` para prevenir violaciones futuras
- **Sin efectos secundarios ocultos**, **sin precondiciones fortalecidas**, **invariantes preservados**

**Beneficio:**

```csharp
// Cualquier Entity puede ser sustituida sin sorpresas
Entity entity = GetEntity(); // Puede ser Session, Community, cualquiera
entity.Raise(new SessionCreatedEvent(id)); // ✓ Funciona en todos los casos
```

### 2. ShapeHierarchy — Composición con interfaz común

**Antes:** `Square` heredaba de `Rectangle` pero rompía el contrato de independencia de propiedades.

**Después:** `IShape` con implementaciones independientes `Rectangle` y `Square`.

- `Rectangle` y `Square` NO tienen relación de herencia
- Ambos implementan `IShape` con el método `CalculateArea()`
- `Rectangle` tiene `Width` y `Height` independientes
- `Square` tiene una sola propiedad `Side`
- Propiedades inmutables (solo lectura) para prevenir mal uso
- **Sin efectos secundarios ocultos**, **contratos claros y separados**

**Beneficio:**

```csharp
// Cualquier IShape puede sustituirse sin cálculos incorrectos
IShape shape = GetShape(); // Rectangle o Square — ambos funcionan correctamente
int area = shape.CalculateArea(); // ✓ Resultado correcto siempre
```

### 3. MemberHierarchy — Composición sobre herencia

**Antes:** Jerarquía de herencia donde `GuestMember` lanzaba excepciones e `InactiveMember` fallaba silenciosamente.

**Después:** Una sola clase `Member` con **diseño basado en capacidades**.

- Todos los Members tienen la misma interfaz
- Las capacidades son consultables: `member.Capabilities.CanVote`
- Las operaciones devuelven `Result` con razones de falla claras — sin excepciones, sin fallas silenciosas
- Métodos de fábrica: `Member.CreateFull()`, `Member.CreateGuest()`, `Member.CreateInactive()`

**Beneficio:**

```csharp
// Puedes sustituir cualquier Member sin verificación de tipo
Member member = GetMember(); // Full, Guest o Inactive — no importa
var result = member.Vote(proposalId, true);
if (result.IsFailure)
    HandleError(result.Error); // Razón de falla clara
```

### 4. SessionHierarchy — Patrón de estrategia con políticas

**Antes:** Jerarquía de herencia donde `WorkshopSession` fortalecía precondiciones y `FreeSession` debilitaba postcondiciones.

**Después:** Una sola clase `Session` con **objetos políticos inyectados**.

- Una sola clase `Session` — sin subtipos
- Las políticas implementan `ISessionPolicy` para hacer cumplir reglas diferentes
- Todas las sesiones respetan el mismo contrato: las operaciones devuelven `Result`
- Métodos de fábrica inyectan políticas apropiadas: `Session.CreateWorkshop()`, `Session.CreatePremium()`, etc.

**Beneficio:**

```csharp
// Puedes sustituir cualquier Session sin sorpresas en tiempo de ejecución
Session session = GetSession(); // Standard, Workshop, Premium, Free — todos funcionan igual
var result = session.RegisterAttendee(memberId);
// Sin excepciones inesperadas, sin verificación de tipo necesaria
```

## Patrones aplicados

| Patrón                           | Dónde                                              | Por qué                                                            |
| -------------------------------- | -------------------------------------------------- | ------------------------------------------------------------------ |
| **Herencia correcta**            | `Entity` → `AuditableEntity`                       | Extiende comportamiento sin modificar contratos base               |
| **Composición sobre herencia**   | `IShape` con tipos independientes                  | Evita jerarquías cuando el comportamiento difiere fundamentalmente |
| **Diseño basado en capacidades** | `Member` con `MemberCapabilities`                  | Evita jerarquías de herencia frágiles                              |
| **Patrón de estrategia**         | `Session` con `ISessionPolicy`                     | Variación de política sin violaciones de subtipo                   |
| **Método de fábrica**            | `Member.CreateGuest()`, `Session.CreateWorkshop()` | Encapsula configuración de política/capacidad                      |
| **Patrón Result**                | Todas las operaciones devuelven `Result`           | Sin excepciones, éxito/falla explícita                             |

## Beneficios obtenidos

- **Substituibilidad**: Cualquier instancia puede usarse en cualquier parte — sin verificación de tipo, sin fallas en tiempo de ejecución
- **Contratos explícitos**: Las capacidades y políticas son consultables y testeables
- **Sin excepciones**: Las operaciones devuelven `Result` con códigos de error claros
- **Extensibilidad**: Agrega nuevas capacidades, políticas o formas sin modificar código existente
- **Testabilidad**: Simula `ISessionPolicy` o `IShape` para probar lógica de forma aislada
- **Cálculos correctos**: Las formas geométricas producen resultados matemáticamente correctos

## 🧪 Aspectos destacados de las pruebas

- Las pruebas pueden trabajar polimórficamente con `Entity` — todas las instancias se comportan consistentemente
- Las pruebas de `IShape` funcionan con cualquier implementación — sin casos especiales
- Las pruebas pueden trabajar polimórficamente con `Member` — todas las instancias se comportan consistentemente
- Las pruebas de Session inyectan diferentes políticas sin duplicar lógica de prueba de Session
- No se necesita try-catch ni verificación de tipo en las pruebas

## Archivos por caso de uso

### EntityHierarchy

| Archivo              | Descripción                                                       |
| -------------------- | ----------------------------------------------------------------- |
| `Entity.cs`          | Clase base con infraestructura de eventos de dominio              |
| `AuditableEntity.cs` | Extiende Entity agregando timestamps sin modificar comportamiento |
| `Session.cs`         | Entidad concreta sealed que hereda correctamente                  |
| `Community.cs`       | Entidad concreta sealed que hereda correctamente                  |
| `Consumers.cs`       | Código de consumo que funciona polimórficamente                   |
| `DomainEvents.cs`    | Eventos de dominio de ejemplo                                     |

### ShapeHierarchy

| Archivo              | Descripción                                   |
| -------------------- | --------------------------------------------- |
| `IShape.cs`          | Interfaz común para todas las formas          |
| `Rectangle.cs`       | Implementación independiente con Width/Height |
| `Square.cs`          | Implementación independiente con Side         |
| `GeometryService.cs` | Servicio que funciona con cualquier IShape    |
| `ShapeCollection.cs` | Colección polimórfica de formas               |
| `ShapeFactory.cs`    | Fábrica para crear formas                     |
| `ColoredShape.cs`    | Decorador que agrega color sin violar LSP     |

### MemberHierarchy

| Archivo     | Descripción                                                |
| ----------- | ---------------------------------------------------------- |
| `Member.cs` | Clase única con diseño basado en capacidades (composición) |

### SessionHierarchy

| Archivo      | Descripción                                                 |
| ------------ | ----------------------------------------------------------- |
| `Session.cs` | Clase única con validación basada en políticas (estrategia) |

## 📖 LSP en la práctica

**El Principio de Sustitución de Liskov establece:**

> "Los objetos de una superclase deben ser reemplazables con objetos de sus subclases sin afectar la corrección."

Nuestra solución logra esto mediante:

1. **Usar herencia solo cuando extiende sin modificar** — `AuditableEntity` agrega comportamiento sin cambiar contratos
2. **Evitar herencia donde el comportamiento difiere** — usar composición en su lugar (`IShape`, `Member`, `Session`)
3. **Honrar contratos** — las operaciones devuelven `Result`, nunca lanzan excepciones inesperadas
4. **Comportamiento consultable** — las capacidades y políticas son explícitas, no ocultas en lógica de subtipo
5. **Interfaces consistentes** — todos los Members/Sessions/Shapes responden a los mismos métodos con contratos predecibles
