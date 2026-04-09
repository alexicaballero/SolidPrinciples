# Ejemplos de Problema ISP

## Qué está mal

Estos ejemplos demuestran **interfaces gordas** que violan el Principio de Segregación de Interfaces:

1. **FatRepositoryInterface.cs**: Una interfaz `ISessionRepository` de 13 métodos que combina consultas, persistencia, caché, estadísticas y auditoría. Implementaciones simples como `InMemorySessionRepository` se ven obligadas a lanzar `NotImplementedException` para los métodos de caché, estadísticas y auditoría que no soportan.

2. **FatMemberOperationsInterface.cs**: Una interfaz `IMemberOperations` de 10 métodos que combina operaciones de visualizador, contribuidor y administrador. `ViewerMemberService` solo puede usar 2 de 10 métodos y devuelve resultados de error para los otros 8.

## Señales de Advertencia

Cómo detectar violaciones ISP en tu código:

- **NotImplementedException** en cuerpos de métodos
- Métodos que devuelven `Result.Failure("No soportado")`
- La interfaz tiene más de 10 métodos cubriendo múltiples responsabilidades
- El nombre de la clase sugiere capacidad limitada (ej.: `InMemoryRepository`) pero implementa una interfaz gorda
- Comentarios como "Esta implementación solo soporta los métodos X, Y, Z"
- Las pruebas unitarias simulan 20 métodos pero solo afirman sobre 2

## Impacto

Qué sucede cuando tienes interfaces gordas:

### Carga de Mantenimiento
- **Cada cambio en la interfaz rompe todas las implementaciones**: Agregar `InvalidateAllCache()` significa actualizar 5 implementaciones de repositorio, incluso aquellas sin caché
- **Niveles de abstracción mezclados**: Invalidación de caché, estadísticas y CRUD básico en una interfaz

### Dificultad para Probar
- **Exceso de mocking**: Las pruebas deben simular métodos irrelevantes
  ```csharp
  // PROBLEMA: Simular una interfaz gorda para una prueba simple
  var mockRepo = Substitute.For<ISessionRepository>();
  mockRepo.GetByIdAsync(sessionId).Returns(session);
  // Aún necesitas simular otros 12 métodos si el SUT los llama accidentalmente
  ```

### Sorpresas en Tiempo de Ejecución
- **NotImplementedException en tiempo de ejecución**: Éxito en tiempo de compilación, falla en tiempo de ejecución
- **API poco clara**: IntelliSense muestra 13 métodos, pero solo 7 son seguros de llamar

### Contratos Violados
- **Sustitución de Liskov violada**: Sustituir `InMemorySessionRepository` por `ISessionRepository` rompe a los llamadores que necesitan caché
- **Documentación implícita**: Los desarrolladores deben leer documentación o código fuente para saber qué métodos son soportados

## Archivos

| Archivo | Descripción |
|------|-------------|
| `FatRepositoryInterface.cs` | Interfaz `ISessionRepository` de 13 métodos con `InMemorySessionRepository` que lanza `NotImplementedException` para operaciones de caché, estadísticas y auditoría |
| `FatMemberOperationsInterface.cs` | Interfaz `IMemberOperations` de 10 métodos con `ViewerMemberService` que devuelve resultados de error para 8 de 10 métodos (operaciones de contribuidor y administrador) |

## Conclusión Clave

**Cuando una interfaz obliga a los implementadores a proporcionar métodos que no soportan, violas ISP.**

Los clientes dependen de una interfaz inflada y obtienen métodos que no pueden llamar de forma segura. Esto rompe la encapsulación, viola LSP y hace que el código sea difícil de probar y mantener.


