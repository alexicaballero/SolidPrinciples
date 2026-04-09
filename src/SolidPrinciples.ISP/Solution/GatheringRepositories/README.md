# Solución: Interfaces Segregadas - Caso Gathering

## La Solución: Interfaces Segregadas

En lugar de una interfaz gorda `IDataRepository` (16+ métodos), Gathering usa interfaces enfocadas.

## Paso 1: Repositorio Base Genérico

[IRepository<T>.cs](IRepository.cs) - Interfaz enfocada y cohesiva:

- ✅ Operaciones CRUD genéricas para cualquier entidad
- ✅ Todos los métodos relacionados con gestión de entidades
- ✅ Sin persistencia mezclada (eso va en IUnitOfWork)

```csharp
public interface IRepository<T> where T : Entity
{
    // Consultas (4 métodos)
    // Verificaciones (2 métodos)
    // Conteo (2 métodos)
    // Comandos (6 métodos)
}
```

## Paso 2: Interfaces Especializadas

### ISessionRepository

[ISessionRepository.cs](ISessionRepository.cs) - Extiende con consultas específicas:

- ✅ Hereda todo de `IRepository<Session>`
- ✅ Agrega solo métodos específicos de Session
- ✅ `GetByCommunityIdAsync()` - solo para Sessions
- ✅ `GetActiveSessionsAsync()` - solo para Sessions

### ICommunityRepository

[ICommunityRepository.cs](ICommunityRepository.cs) - Sin métodos adicionales:

- ✅ Solo hereda `IRepository<Community>`
- ✅ La interfaz base es suficiente
- ✅ Sin contaminar con métodos de Session

**Patrón clave**: No todas las interfaces necesitan agregar métodos.
A veces heredar la interfaz base es suficiente.

## Paso 3: Persistencia Separada

[IUnitOfWork.cs](IUnitOfWork.cs) - Una interfaz, una responsabilidad:

- ✅ Solo 1 método: `SaveChangesAsync()`
- ✅ Segregado de las consultas del repositorio
- ✅ Ejemplo definitivo de ISP

## Paso 4: Almacenamiento Enfocado

[IImageStorageService.cs](IImageStorageService.cs) - Solo lo necesario:

- ✅ Solo 2 métodos: `UploadImageAsync()` y `DeleteImageAsync()`
- ✅ Sin métodos de documento, video o archivo genérico
- ✅ Fácil de implementar, mockear e intercambiar

## Paso 5: Handler con Dependencias Precisas

[CreateCommunityCommandHandler.cs](CreateCommunityCommandHandler.cs) - ISP en acción:

```csharp
public CreateCommunityCommandHandler(
    ICommunityRepository communityRepository,  // Solo Community
    IUnitOfWork unitOfWork,                    // Solo persistencia
    IImageStorageService imageStorageService)  // Solo imágenes
```

## Comparación: Antes vs Ahora

| Aspecto                 | Problema         | Solución               |
| ----------------------- | ---------------- | ---------------------- |
| **Dependencias**        | 1 interfaz gorda | 3 interfaces enfocadas |
| **Métodos disponibles** | 16+              | 2 + 1 + 2 = 5          |
| **Métodos usados**      | 2 de 16 (12.5%)  | 5 de 5 (100%)          |
| **Acoplamiento**        | Alto             | Mínimo                 |
| **Mock tests**          | 16+ métodos      | 5 métodos              |
| **Intención**           | Confusa          | Clara                  |

## Beneficios de ISP

### 1. Acoplamiento Reducido

Cambios en `ISessionRepository` no afectan a `CreateCommunityCommandHandler`.

### 2. Pruebas Más Simples

```csharp
// Mockear 3 interfaces pequeñas vs 1 interfaz gorda
var mockCommunityRepo = new Mock<ICommunityRepository>();
var mockUnitOfWork = new Mock<IUnitOfWork>();
var mockImageStorage = new Mock<IImageStorageService>();
// Total: 5 métodos vs 16+ métodos
```

### 3. Intención Más Clara

El constructor dice exactamente qué hace el handler:

- Gestiona communities
- Persiste cambios
- Maneja imágenes

### 4. Evolución Independiente

Agregar `GetUpcomingSessionsAsync()`:

- ✅ Se agrega a `ISessionRepository`
- ✅ No afecta a `ICommunityRepository`
- ✅ No afecta a handlers de Community

### 5. DI Preciso

```csharp
services.AddScoped<ISessionRepository, SessionRepository>();
services.AddScoped<ICommunityRepository, CommunityRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IImageStorageService, AzureBlobStorageService>();
```

## Patrón Clave: Segregación + Composición

Interfaces pequeñas y enfocadas que se componen según las necesidades del cliente:

```csharp
// Handler simple: solo Community + persistencia
class CreateCommunityHandler(
    ICommunityRepository repo,
    IUnitOfWork uow) { }

// Handler complejo: Session + Community + imágenes + persistencia
class CreateSessionHandler(
    ISessionRepository sessionRepo,
    ICommunityRepository communityRepo,
    IImageStorageService imageStorage,
    IUnitOfWork uow) { }
```

Cada handler depende **solo de lo que necesita**.

## Referencia

Código basado en:

- [Gathering backend](https://github.com/alexicaballero/gathering/tree/main/backend/src)
- [Artículo ISP](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/interface-segregation) -
  Sección: "Caso de Estudio: Interfaces de Repositorio en Gathering"
