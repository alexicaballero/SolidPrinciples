# Infraestructura (Implementaciones Concretas)

Esta carpeta contiene las **implementaciones concretas** de las abstracciones definidas en `Abstractions/`.

## Propósito educativo

Estas clases de infraestructura:

- **Implementan** las interfaces (abstracciones)
- **Dependen** de las abstracciones (inversión de dependencia)
- **NO son conocidas** directamente por `CommunityService`

## Contenido

### Implementaciones de `ICommunityRepository`

#### `EFCoreCommunityRepository.cs` (Producción)

- Persistencia con Entity Framework Core
- Accede a base de datos SQL Server
- Registrada en DI: `services.AddScoped<ICommunityRepository, EFCoreCommunityRepository>()`

#### `InMemoryCommunityRepository.cs` (Pruebas)

- Almacenamiento en memoria con `List<Community>`
- Perfecta para pruebas unitarias rápidas
- Sin dependencias externas

### Implementaciones de `ILogger`

#### `FileSystemLogger.cs` (Producción básica)

- Escribe logs en archivo de texto
- Usa I/O del sistema de archivos

#### `ConsoleLogger.cs` (Desarrollo)

- Escribe logs en la consola
- Útil para debugging local

#### `AzureAppInsightsLogger.cs` (Producción empresarial)

- Envía telemetría a Azure Application Insights
- Permite monitoreo centralizado y análisis

## Intercambiabilidad (poder de DIP)

**Producción:**

```csharp
services.AddScoped<ICommunityRepository, EFCoreCommunityRepository>();
services.AddSingleton<ILogger, AzureAppInsightsLogger>();
```

**Desarrollo:**

```csharp
services.AddScoped<ICommunityRepository, EFCoreCommunityRepository>();
services.AddSingleton<ILogger, ConsoleLogger>();
```

**Pruebas:**

```csharp
var repository = new InMemoryCommunityRepository();
var logger = new ConsoleLogger();
var service = new CommunityService(repository, logger);
```

`CommunityService` **no cambia** — solo cambian las implementaciones inyectadas.

## Inversión de Dependencia

```
CommunityService → ICommunityRepository ← EFCoreCommunityRepository
                                        ← InMemoryCommunityRepository

CommunityService → ILogger ← FileSystemLogger
                           ← ConsoleLogger
                           ← AzureAppInsightsLogger
```

Las flechas apuntan **HACIA las abstracciones**, demostrando la inversión.
