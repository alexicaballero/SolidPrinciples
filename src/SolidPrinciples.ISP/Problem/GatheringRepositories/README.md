# Problema: Interfaz de Repositorio Gorda - Caso Gathering

## El Problema

[IDataRepository.cs](IDataRepository.cs) es una interfaz gorda con **16+ métodos** que mezcla:

- Operaciones de Session (7 métodos)
- Operaciones de Community (5 métodos)
- Operaciones de Resource (3 métodos)
- Persistencia (1 método)

## La Violación

[CreateCommunityCommandHandler.cs](CreateCommunityCommandHandler.cs) demuestra el problema:

- Depende de `IDataRepository` (16+ métodos disponibles)
- Usa solo **2 métodos**: `AddCommunity()` y `SaveChangesAsync()`
- Los otros **14+ métodos** = acoplamiento innecesario

## Consecuencias

### 1. Acoplamiento Innecesario

El handler de Community está acoplado a operaciones de Session y Resource que nunca usa.

### 2. Difícil de Probar

```csharp
// Mock requiere configurar 16+ métodos
var mockRepo = new Mock<IDataRepository>();
mockRepo.Setup(x => x.AddCommunity(It.IsAny<Community>()));
mockRepo.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
// ... ¿y los otros 14 métodos? 🤷
```

### 3. Amplificación de Cambios

Agregar `GetUpcomingSessionsAsync()` a `IDataRepository`:

- ✓ Necesario para SessionQueryHandler
- Fuerza recompilación de CreateCommunityCommandHandler
- Fuerza recompilación de todos los handlers de Community

### 4. SRP Violado

La interfaz tiene múltiples responsabilidades:

- Gestionar Sessions
- Gestionar Communities
- Gestionar Resources
- Manejar Persistencia

## Señales de Advertencia

✋ **Si un cliente depende de una interfaz con 16+ métodos pero usa solo 2, ISP está violado.**

## Comparación

| Aspecto   | Handler                       | Necesita  | Obtiene     | Acoplamiento Extra |
| --------- | ----------------------------- | --------- | ----------- | ------------------ |
| Community | CreateCommunityCommandHandler | 2 métodos | 16+ métodos | 14+ métodos        |

## Referencia

Ver [artículo ISP](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/interface-segregation) -
Sección: "Caso de Estudio: Interfaces de Repositorio en Gathering - La Violación"
