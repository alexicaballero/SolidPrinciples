# Caso de Uso 1: Creación de Sesiones

## El Problema: Clase Dios SessionManager

La clase `SessionManager` viola SRP al manejar **5 responsabilidades diferentes**:

1. **Validación de entrada** - Verifica que título, speaker y fecha sean válidos
2. **Persistencia** - Escribe directamente a SQL usando ADO.NET
3. **Notificación** - Envía correos electrónicos vía SMTP
4. **Registro** - Escribe logs a archivos del sistema
5. **Orquestación** - Coordina el flujo del caso de uso

### 🚩 Señales de Advertencia

```csharp
// Nombre vago que esconde múltiples responsabilidades
public sealed class SessionManager

// Importa infraestructura de diferentes capas
using System.Data;           // SQL
using System.Net.Mail;       // SMTP
using System.IO;             // File system

// Constructor complejo con dependencias de infraestructura
public SessionManager(
    string connectionString,  // Database
    string smtpHost,          // Email
    int smtpPort,             // Email
    string logFilePath)       // Logging
```

### 💥 Razones para Cambiar

Esta clase debe modificarse cuando:

- Cambian las reglas de validación (nueva política de títulos)
- Cambia el esquema de base de datos (nueva columna)
- Cambia la plantilla de correo electrónico (nuevo formato)
- Cambia el formato de registro (JSON → structured logging)
- Cambia el canal de notificación (agregar Slack, Teams)
- Migramos a un nuevo ORM (EF Core)

### 🧪 Pesadilla de Pruebas

Para probar esta clase necesitas:

```csharp
// Ambiente de prueba complejo
✓ Base de datos SQL en ejecución con esquema correcto
✓ Servidor SMTP configurado (o mock complejo)
✓ Sistema de archivos escribible
✓ Permisos de red
✓ Configuración de múltiples dependencias
```

### 📖 Ver la Solución

Revisa `../../Solution/SessionCreation/` para ver cómo SRP divide esto en clases enfocadas:

- `Session.cs` - **Solo** estado de dominio y validación
- `CreateSessionCommandHandler.cs` - **Solo** orquestación
- `ISessionRepository.cs` - **Solo** interfaz de persistencia
- `INotificationService.cs` - **Solo** interfaz de notificación

Cada clase tiene **una única razón para cambiar**.
