# OCP — Solución (Correcto)

## Cómo se arregla

Ambos escenarios se refactorizan usando el **patrón Estrategia** combinado con **inyección de dependencias**, haciendo el código **abierto para extensión** y **cerrado para modificación**.

### Exportación de sesión — Patrón Estrategia

```
ISessionExportStrategy          ← abstracción
├── JsonSessionExporter         ← una clase por formato
├── CsvSessionExporter
└── XmlSessionExporter

SessionExportService            ← orquestador (nunca cambia)
    ctor(IEnumerable<ISessionExportStrategy>)
```

**¿Quieres agregar PDF?**
1. Crea `PdfSessionExporter : ISessionExportStrategy`
2. Regístralo en DI
3. Listo — `SessionExportService` **nunca se toca**

### Notificaciones — Arquitectura de complementos

```
INotificationChannel            ← abstracción
├── EmailNotificationChannel    ← una clase por canal
├── SmsNotificationChannel
└── SlackNotificationChannel

NotificationService             ← orquestador (nunca cambia)
    ctor(IEnumerable<INotificationChannel>)
```

**¿Quieres agregar Teams?**
1. Crea `TeamsNotificationChannel : INotificationChannel`
2. Regístralo en DI
3. Listo — `NotificationService` **nunca se toca**

## Beneficios

| Beneficio | Explicación |
|-----------|-------------|
| **Sin necesidad de modificación** | Los nuevos formatos/canales se agregan creando nuevas clases |
| **Prueba independiente** | Cada estrategia se prueba de forma aislada |
| **Sin riesgo para código existente** | Agregar PDF no puede romper JSON |
| **Amigable con complementos** | Las estrategias pueden vivir en ensamblados separados |
| **Nativa de DI** | La inyección de `IEnumerable<T>` es un patrón estándar de .NET |

## 🧪 Destacados de pruebas

- Cada estrategia tiene sus **propias pruebas enfocadas** — una prueba JSON nunca toca código CSV
- El `SessionExportService` se prueba con **estrategias inyectadas**
- La extensibilidad se prueba inyectando una **estrategia stub en tiempo de ejecución** (p. ej., `MarkdownSessionExporterStub`)
- Agregar un nuevo formato agrega una **nueva clase de prueba** — las pruebas existentes no se modifican

## 🔗 Ejemplo de registro de DI

```csharp
// En Program.cs o Startup
services.AddSingleton<ISessionExportStrategy, JsonSessionExporter>();
services.AddSingleton<ISessionExportStrategy, CsvSessionExporter>();
services.AddSingleton<ISessionExportStrategy, XmlSessionExporter>();
services.AddSingleton<SessionExportService>();

services.AddSingleton<INotificationChannel, EmailNotificationChannel>();
services.AddSingleton<INotificationChannel, SmsNotificationChannel>();
services.AddSingleton<INotificationChannel, SlackNotificationChannel>();
services.AddSingleton<NotificationService>();
