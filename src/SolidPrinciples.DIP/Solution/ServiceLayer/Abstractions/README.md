# Abstracciones (Interfaces)

Esta carpeta contiene las **abstracciones** (interfaces) que definen los contratos para las dependencias del `CommunityService`.

## Propósito educativo

En el Principio de Inversión de Dependencias (DIP):

- **Los servicios de alto nivel NO deben depender de la infraestructura de bajo nivel**
- **Ambos deben depender de abstracciones**

## Contenido

### `ICommunityRepository.cs`

Contrato para la persistencia de comunidades. Define:

- `AddAsync()` - Agregar una comunidad
- `GetByIdAsync()` - Recuperar una comunidad por ID

**Implementaciones disponibles:**

- `EFCoreCommunityRepository` - Persistencia con Entity Framework Core (producción)
- `InMemoryCommunityRepository` - Repositorio en memoria (pruebas)

### `ILogger.cs`

Contrato para el registro de eventos. Define:

- `Log()` - Registrar un mensaje

**Implementaciones disponibles:**

- `FileSystemLogger` - Escribe en archivo de texto
- `ConsoleLogger` - Escribe en consola (desarrollo)
- `AzureAppInsightsLogger` - Envía telemetría a Azure (producción)

## Flujo de dependencia (DIP)

```
CommunityService (alto nivel)
        ↓ depende de
    ICommunityRepository + ILogger (abstracciones)
        ↑ implementadas por
EFCoreCommunityRepository + FileSystemLogger (bajo nivel)
```

## Beneficio

Al depender de `ICommunityRepository` y `ILogger`:

- ✅ El servicio es **testeable** sin base de datos ni sistema de archivos
- ✅ Las implementaciones son **intercambiables** (EF Core → Dapper, File → Azure)
- ✅ El servicio se enfoca en **lógica de negocio**, no en infraestructura
