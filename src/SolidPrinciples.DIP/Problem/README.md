# Ejemplos de Problemas DIP

## Qué Está Mal

Estos ejemplos demuestran **dependencias directas** que violan el Principio de Inversión de Dependencias:

1. **DirectDependenciesInHandler.cs**: Un `CreateSessionHandler` que instancia directamente `SqlSessionRepository` y `EmailNotificationSender` usando `new`. El controlador (lógica empresarial de alto nivel) depende de clases concretas de infraestructura (detalles de bajo nivel). No se puede hacer pruebas unitarias sin una base de datos real y servidor de correo.

2. **ConcreteDependenciesInService.cs**: Un `CommunityService` que instancia directamente `EFCoreCommunityRepository` y `FileSystemLogger` en su constructor. El servicio está estrechamente acoplado a EF Core e I/O del sistema de archivos. No se puede probar sin base de datos y sistema de archivos.

## Señales de Alerta

Cómo detectar violaciones de DIP en tu código:

- **Palabra clave `new` en lógica empresarial**: `var repository = new SqlSessionRepository();`
- **Constructores sin parámetros ocultando dependencias**: Constructor llama a `new` internamente
- **Sentencias import para infraestructura**: `using System.Data.SqlClient;` en capa de aplicación
- **Tipos concretos hardcodeados**: Parámetros de método tipados como clases concretas, no interfaces
- **Pruebas unitarias requieren base de datos/red/sistema de archivos**: No puedes ejecutar pruebas sin dependencias externas
- **Singletons estáticos**: `Logger.Instance`, `DatabaseConnection.Current`

## Impacto

Qué sucede cuando tienes dependencias directas:

### Pesadilla de Pruebas
- **No se pueden hacer pruebas unitarias**: La lógica empresarial requiere base de datos, servidor SMTP, sistema de archivos
- **Pruebas lentas**: Las pruebas de integración toman segundos/minutos en lugar de milisegundos
- **Pruebas frágiles**: Los cambios de esquema de base de datos rompen las pruebas
- **No se pueden simular**: No hay forma de inyectar dobles de prueba

Ejemplo de código que no es testeable:
```csharp
// PROBLEMA: ¿Cómo pruebas esto sin una base de datos real?
public async Task<Result> Handle()
{
  var repository = new SqlSessionRepository(); // Dependencia hardcodeada
  await repository.SaveAsync(session);
}
```

### Acoplamiento Estrecho
- **Difícil cambiar implementaciones**: Cambiar SQL → MongoDB requiere editar código del controlador
- **No se puede evolucionar independientemente**: Los cambios de infraestructura fuerzan cambios en la lógica empresarial
- **Alto nivel depende de bajo nivel**: La flecha de dependencia apunta en la dirección equivocada

### Inflexibilidad de Implementación
- **No se puede configurar por entorno**: Dev, staging, prod todos usan la misma implementación hardcodeada
- **Sin banderas de funcionalidad**: No puedes alternar entre implementaciones en tiempo de ejecución
- **La implementación requiere recompilación**: Cambiar proveedor de correo requiere reconstruir capa de aplicación

### Arquitectura Violada
- **Capa de aplicación importa capa de infraestructura**: Viola reglas de límite de arquitectura limpia
- **Dependencias circulares**: Módulos de alto y bajo nivel entrelazados
- **Sin arquitectura de plugin**: No puedes agregar nuevas implementaciones sin modificar código existente

## Archivos

| Archivo | Descripción |
|---------|-------------|
| `DirectDependenciesInHandler.cs` | `CreateSessionHandler` instancia directamente `SqlSessionRepository` y `EmailNotificationSender` con `new`. El controlador depende de clases concretas de infraestructura. No se puede hacer pruebas unitarias. |
| `ConcreteDependenciesInService.cs` | `CommunityService` instancia directamente `EFCoreCommunityRepository` y `FileSystemLogger` en su constructor. El servicio está estrechamente acoplado a EF Core y sistema de archivos. No se puede probar sin base de datos. |

## Conclusión Clave

**Cuando la lógica empresarial de alto nivel depende directamente de infraestructura de bajo nivel, violas DIP.**

Los módulos de alto nivel deben depender de abstracciones (interfaces), no de implementaciones concretas. Las dependencias deben inyectarse, no instanciarse internamente.

Esta violación hace que el código sea:
- **No testeable** (requiere infraestructura real)
- **Inflexible** (no puedes cambiar implementaciones)
- **Estrechamente acoplado** (los cambios se propagan en cascada)
- **Difícil de mantener** (la lógica empresarial conoce detalles de infraestructura)


