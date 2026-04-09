# Solución — SRP Aplicado

## Qué se corrigió

Ahora cada clase tiene **exactamente una razón para cambiar**. La entidad de sesión solo gestiona el estado del dominio y la validación. El controlador de comandos solo orquesta el flujo del caso de uso. La persistencia, notificación, formateo y entrega están cada una detrás de su propia interfaz, implementada por clases dedicadas.

## Patrones aplicados

- **Método de Fábrica**: `Session.Create()` encapsula la construcción de entidades y la validación
- **Eventos de Dominio**: `SessionCreatedDomainEvent` desvincula los efectos secundarios de la lógica de entidades
- **Controlador de Comandos (CQRS)**: `CreateSessionCommandHandler` orquesta sin implementar preocupaciones
- **Patrón Estrategia**: `IReportFormatter` permite intercambiar formateadores HTML/CSV/PDF
- **Inyección de Dependencias**: Todas las dependencias se inyectan mediante el constructor — totalmente reemplazables

## Beneficios obtenidos

- **Testabilidad**: La entidad de sesión se prueba con cero infraestructura. El controlador se prueba con dependencias simuladas. El formateador se prueba en aislamiento puro.
- **Extensibilidad**: Agregar notificaciones de Slack = nueva implementación de `INotificationService`. Agregar reportes PDF = nueva implementación de `IReportFormatter`. Sin cambios en el código existente.
- **Mantenibilidad**: Cambiar la plantilla de correo electrónico nunca riesga romper la lógica de base de datos. Cambiar el esquema de SQL nunca afecta el código de notificación.
- **Legibilidad**: Cada nombre de clase describe precisamente qué hace. Sin nombres vagos como "Manager" o "Service".

## Archivos

| Archivo | Responsabilidad |
|------|---------------|
| `Session.cs` | Entidad de dominio — gestión de estado y validación de invariantes |
| `SessionErrors.cs` | Definiciones de error para operaciones de sesión |
| `SessionCreatedDomainEvent.cs` | Evento de dominio — comunica intención sin efectos secundarios |
| `CreateSessionCommand.cs` | DTO de comando — representa la intención de crear una sesión |
| `CreateSessionCommandHandler.cs` | Orquestador — coordina el flujo de trabajo de creación de sesión |
| `ISessionRepository.cs` | Abstracción de persistencia |
| `INotificationService.cs` | Abstracción de notificación |
| `IUnitOfWork.cs` | Abstracción de gestión de transacciones |
| `CommunityReportGenerator.cs` | Orquestador de reportes — delega en colaboradores |
| `HtmlReportFormatter.cs` | Formateo HTML (responsabilidad única) |
| `IReportDataProvider.cs` | Abstracción de recuperación de datos |
| `IReportFormatter.cs` | Abstracción de formateo |
| `IReportSender.cs` | Abstracción de entrega |


