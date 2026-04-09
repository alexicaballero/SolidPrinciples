# Abstracciones (Interfaces)

Esta carpeta contiene las **abstracciones** (interfaces) que definen los contratos para las dependencias del `CreateSessionHandler`.

## Propósito educativo

En el Principio de Inversión de Dependencias (DIP):

- **Los módulos de alto nivel NO deben depender de módulos de bajo nivel**
- **Ambos deben depender de abstracciones**

## Contenido

### `ISessionRepository.cs`

Contrato para la persistencia de sesiones. Define los métodos que cualquier implementación de repositorio debe proporcionar:

- `SaveAsync()` - Guardar una sesión
- `GetByIdAsync()` - Recuperar una sesión por ID

**Implementaciones disponibles:**

- `SqlSessionRepository` - Persistencia en SQL Server (producción)
- `InMemorySessionRepository` - Repositorio en memoria (pruebas)

### `INotificationSender.cs`

Contrato para el envío de notificaciones. Define el método:

- `SendAsync()` - Enviar una notificación a un destinatario

**Implementaciones disponibles:**

- `EmailNotificationSender` - Envío por SMTP (producción)
- `ConsoleNotificationSender` - Salida por consola (desarrollo/pruebas)

## Flujo de dependencia (DIP)

```
CreateSessionHandler (alto nivel)
        ↓ depende de
    ISessionRepository (abstracción)
        ↑ implementada por
SqlSessionRepository (bajo nivel)
```

La flecha de dependencia apunta **hacia la abstracción**, no hacia los detalles de implementación.

## Beneficio

Al depender de abstracciones en lugar de implementaciones concretas:

- ✅ El handler es **testeable** con mocks/stubs
- ✅ Las implementaciones son **intercambiables** sin modificar el handler
- ✅ El handler se enfoca en **lógica de negocio**, no en detalles de infraestructura
