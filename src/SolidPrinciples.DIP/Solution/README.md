# Ejemplos de Soluciones DIP

## Qué Fue Arreglado

Estos ejemplos demuestran **inversión de dependencias** — módulos de alto nivel dependen de abstracciones, no de implementaciones concretas:

1. **InvertedDependenciesInHandler.cs**: `CreateSessionHandler` depende de abstracciones `ISessionRepository` e `INotificationSender`, inyectadas vía constructor. Se proporcionan múltiples implementaciones:
   - `SqlSessionRepository` (producción)
   - `InMemorySessionRepository` (pruebas)
   - `EmailNotificationSender` (producción)
   - `ConsoleNotificationSender` (pruebas)

2. **InvertedDependenciesInService.cs**: `CommunityService` depende de abstracciones `ICommunityRepository` e `ILogger`, inyectadas vía constructor. Se proporcionan múltiples implementaciones:
   - `EFCoreCommunityRepository` (producción - EF Core)
   - `InMemoryCommunityRepository` (pruebas)
   - `FileSystemLogger` (registro basado en archivos)
   - `ConsoleLogger` (registro en consola)
   - `AzureAppInsightsLogger` (producción - Azure)

## Patrones Aplicados

### 1. Inyección de Dependencias (Inyección vía Constructor)
- Las dependencias se pasan como parámetros de constructor
- Sin palabra clave `new` en lógica empresarial
- Las clases declaran lo que necesitan (dependencias) en lugar de crearlas

Antes (Problema):
```csharp
public CreateSessionHandler()
{
  _repository = new SqlSessionRepository(); // Instanciación directa
}
```

Después (Solución):
```csharp
public CreateSessionHandler(ISessionRepository repository, INotificationSender sender)
{
  _repository = repository; // Dependencia inyectada
  _notificationSender = sender;
}
```

### 2. Abstracciones de Interfaz
- Define contratos (interfaces) para dependencias
- La lógica de alto nivel depende de interfaces
- La lógica de bajo nivel implementa interfaces

```csharp
// Abstracción propiedad del módulo de alto nivel (capa de Aplicación)
public interface ISessionRepository
{
  Task SaveAsync(Session session, CancellationToken cancellationToken);
}

// Implementación de bajo nivel (capa de Infraestructura)
public sealed class SqlSessionRepository : ISessionRepository { /* ... */ }
```

### 3. Múltiples Implementaciones
- Misma interfaz, diferentes implementaciones
- Intercambia en tiempo de ejecución vía contenedor DI
- Usa en-memoria/simuladas para pruebas, producción para implementación

```csharp
// Configuración de producción
services.AddScoped<ISessionRepository, SqlSessionRepository>();

// Configuración de prueba
var repository = new InMemorySessionRepository();
var handler = new CreateSessionHandler(repository, ...);
```

### 4. Inversión del Flujo de Dependencia
- Tradicional: Alto nivel → Bajo nivel (violación)
- Invertido: Alto nivel → Abstracción ← Bajo nivel (correcto)

```
CreateSessionHandler (Aplicación, alto nivel)
    ↓ depende de ↓
ISessionRepository (Aplicación, abstracción)
    ↑ implementado por ↑
SqlSessionRepository (Infraestructura, bajo nivel)
```

## Beneficios Obtenidos

### Testabilidad Unitaria
Antes (Problema):
```csharp
[Fact]
public void Test_CreateSession()
{
  var handler = new CreateSessionHandler();
  // Requiere base de datos SQL real y servidor SMTP
  handler.HandleAsync(...);
}
```

Después (Solución):
```csharp
[Fact]
public void Test_CreateSession()
{
  var repository = new InMemorySessionRepository(); // En memoria, sin base de datos
  var sender = new ConsoleNotificationSender();     // Consola, sin SMTP
  var handler = new CreateSessionHandler(repository, sender);
  
  handler.HandleAsync(...);
  // La prueba se ejecuta en milisegundos, sin dependencias externas
}
```

### Flexibilidad (Implementaciones Intercambiables)
Antes: Cambiar SQL → MongoDB requiere editar `CreateSessionHandler`

Después: Registra diferente implementación en contenedor DI
```csharp
// SQL (original)
services.AddScoped<ISessionRepository, SqlSessionRepository>();

// Cambiar a MongoDB (sin cambios de código en controlador)
services.AddScoped<ISessionRepository, MongoDbSessionRepository>();
```

### Desacoplamiento (Separación de Preocupaciones)
Antes: El controlador importa `System.Data.SqlClient`, conoce sobre SQL

Después: El controlador solo conoce sobre `ISessionRepository`, sin detalles de infraestructura

### Arquitectura en Capas (Arquitectura Limpia)
- **Capa de Aplicación** (alto nivel): Define interfaz `ISessionRepository`
- **Capa de Infraestructura** (bajo nivel): Implementa `SqlSessionRepository`
- Flecha de dependencia: Infraestructura → Aplicación (dirección correcta)

### Configuración Por Entorno
```csharp
// Desarrollo: Almacenamiento en memoria rápido + registro en consola
if (builder.Environment.IsDevelopment())
{
  services.AddScoped<ISessionRepository, InMemorySessionRepository>();
  services.AddSingleton<ILogger, ConsoleLogger>();
}

// Producción: Base de datos SQL + Azure Application Insights
else
{
  services.AddScoped<ISessionRepository, SqlSessionRepository>();
  services.AddSingleton<ILogger, AzureAppInsightsLogger>();
}

// La lógica empresarial (CreateSessionHandler, CommunityService) no cambia
```

## Archivos

| Archivo | Descripción |
|---------|-------------|
| `InvertedDependenciesInHandler.cs` | `CreateSessionHandler` depende de abstracciones `ISessionRepository` e `INotificationSender` (inyección vía constructor). Incluye múltiples implementaciones: `SqlSessionRepository`, `InMemorySessionRepository`, `EmailNotificationSender`, `ConsoleNotificationSender`. |
| `InvertedDependenciesInService.cs` | `CommunityService` depende de abstracciones `ICommunityRepository` e `ILogger` (inyección vía constructor). Incluye múltiples implementaciones: `EFCoreCommunityRepository`, `InMemoryCommunityRepository`, `FileSystemLogger`, `ConsoleLogger`, `AzureAppInsightsLogger`. |

## Conclusión Clave

**Depende de abstracciones (interfaces), no de implementaciones concretas. Inyecta dependencias vía constructor.**

La inversión de dependencias hace que el código sea:
- **Testeable** (inyecta simuladas/en-memoria)
- **Flexible** (intercambia implementaciones)
- **Desacoplado** (alto nivel no conoce detalles de bajo nivel)
- **Mantenible** (los cambios no se propagan en cascada)

## 📖 Lectura Adicional

- [Artículo de Principio de Inversión de Dependencias](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/dependency-inversion)
- [Arquitectura Limpia](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) por Robert C. Martin
- [Proyecto de Referencia Gathering](https://github.com/alexicaballero/gathering/tree/main/backend) — Ver `Application/Abstractions` para interfaces, `Infrastructure/Persistence` para implementaciones
- [Docs de Microsoft: Inyección de Dependencias en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)


