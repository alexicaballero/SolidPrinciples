# Ejemplos de Solución ISP

## Qué se Corrigió

Estos ejemplos demuestran **segregación de interfaces** — dividir interfaces gordas en contratos enfocados y basados en roles:

1. **SegregatedRepositoryInterfaces.cs**: La interfaz gorda `ISessionRepository` se divide en 5 interfaces enfocadas:
   - `ISessionReader` (consultas)
   - `ISessionWriter` (persistencia)
   - `ISessionCache` (caché opcional)
   - `ISessionStatistics` (estadísticas opcionales)
   - `ISessionAudit` (auditoría opcional)

   Las implementaciones eligen solo las interfaces que soportan. `InMemorySessionRepository` implementa `ISessionReader` e `ISessionWriter`, pero NO `ISessionCache`, `ISessionStatistics` ni `ISessionAudit`.

2. **SegregatedMemberOperationsInterfaces.cs**: La interfaz gorda `IMemberOperations` se divide en 3 interfaces basadas en roles:
   - `IMemberViewer` (solo lectura: ver miembros, ver sesiones)
   - `ISessionContributor` (crear/editar/retirar propias sesiones)
   - `ICommunityAdmin` (aprobar/rechazar/eliminar/banear/promover)

   Los servicios implementan solo los roles que representan. `ViewerMemberService` implementa solo `IMemberViewer`. `ContributorService` implementa `IMemberViewer` e `ISessionContributor`. `AdminService` implementa las tres interfaces.

## Patrones Aplicados

### 1. Segregación de Interfaces Basada en Roles
- Dividir por **necesidad del cliente**: ¿Qué usa realmente este cliente?
- Dividir por **capacidad**: ¿Qué puede realmente proporcionar esta implementación?
- Dividir por **nivel de permisos**: Visualizador vs. Contribuidor vs. Administrador

### 2. Composición de Interfaces
- Una única clase puede implementar **múltiples interfaces enfocadas**
- Componer capacidades en lugar de heredar de una clase base gorda
- Ejemplo: `AdminService : IMemberViewer, ISessionContributor, ICommunityAdmin`

### 3. Inyección de Dependencias con Interfaces Mínimas
- Los clientes solicitan solo la interfaz que necesitan
- Ejemplo: Un dashboard solicita `ISessionReader`, no el `ISessionRepository` completo
- Simular solo los métodos bajo prueba

## Beneficios Obtenidos

### No Más NotImplementedException
Antes (Problema):
```csharp
public Task InvalidateCacheAsync(Guid sessionId, ...)
{
  throw new NotImplementedException("El repositorio en memoria no utiliza caché.");
}
```

Después (Solución):
```csharp
// La clase simplemente no implementa ISessionCache
```

### Contratos Más Claros
Antes: `ISessionRepository` — ¿qué hace? ¡Todo!
Después: `ISessionReader`, `ISessionWriter` — nombres precisos y enfocados

### Pruebas Más Fáciles
Antes:
```csharp
var mockRepo = Substitute.For<ISessionRepository>(); // 13 métodos
```

Después:
```csharp
var mockReader = Substitute.For<ISessionReader>(); // 4 métodos
```

### Capacidades Componibles
Antes: Admin hereda de una clase base que lanza excepciones para métodos que no soporta
Después: Admin implementa tres interfaces enfocadas y proporciona implementaciones completas para todas

### Evolución Flexible
- ¿Agregar nuevas características de admin? Extender `ICommunityAdmin` — visualizadores y contribuidores no se ven afectados
- ¿Agregar caché a un repositorio específico? Implementar `ISessionCache` — otros repos no cambian

## Archivos

| Archivo | Descripción |
|------|-------------|
| `SegregatedRepositoryInterfaces.cs` | Divide el repositorio gordo en `ISessionReader`, `ISessionWriter`, `ISessionCache`, `ISessionStatistics`, `ISessionAudit`. Incluye `InMemorySessionRepository` (Reader + Writer) y `CachedSessionRepository` (Reader + Writer + Cache) |
| `SegregatedMemberOperationsInterfaces.cs` | Divide la interfaz de operaciones gorda en `IMemberViewer`, `ISessionContributor`, `ICommunityAdmin`. Incluye `ViewerMemberService` (solo Viewer), `ContributorService` (Viewer + Contributor), `AdminService` (las tres) |

## Conclusión Clave

**Los clientes deben depender de la interfaz más pequeña que necesiten. Las implementaciones deben proporcionar solo lo que realmente pueden soportar.**

La segregación de interfaces hace que el código sea más testeable, mantenible y componible al eliminar dependencias forzadas y sorpresas en tiempo de ejecución.

## 📖 Lectura Adicional

- [Artículo sobre el Principio de Segregación de Interfaces](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/interface-segregation)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) por Robert C. Martin
- [Proyecto de Referencia Gathering](https://github.com/alexicaballero/gathering/tree/main/backend) — Ver `Application/Abstractions` para ejemplos de interfaces


