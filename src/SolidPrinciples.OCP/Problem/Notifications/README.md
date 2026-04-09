# Caso de Uso 2: Canales de Notificación

## El Problema: Cadena If-Else en Canal

La clase `NotificationDispatcher` viola OCP porque **debe ser modificada** cada vez que necesitas agregar un nuevo canal de notificación.

### 🚩 El Anti-Patrón: If-Else en Tipo

```csharp
public string Send(string channel, string recipient, string message)
{
    if (channel == "email")
        return /* lógica SMTP */;
    else if (channel == "sms")
        return /* lógica SMS */;
    else if (channel == "slack")
        return /* lógica Slack */;

    // ¿Necesitas Teams? Debes modificar ESTA clase
    // ¿Necesitas Push? Debes modificar ESTA clase
    // ¿Necesitas Webhook? Debes modificar ESTA clase
    else
        throw new NotSupportedException();
}
```

### 💥 Por Qué Esto Viola OCP

**NO está cerrada para modificación**:

- Agregar Teams → Editar `NotificationDispatcher.cs`
- Agregar Webhook → Editar `NotificationDispatcher.cs`
- Agregar Push → Editar `NotificationDispatcher.cs`

**NO está abierta para extensión**:

- No puedes agregar un canal sin tocar código de email/sms/slack
- Riesgo de romper canales existentes al agregar nuevos
- El método `Send` crece sin límite

### 🧪 Acoplamiento Peligroso

```csharp
// Este método conoce CADA implementación
public string Send(string channel, ...)
{
    // Conoce detalles de SMTP
    if (channel == "email") { /* envío SMTP */ }

    // Conoce detalles de SMS (trunca a 160 chars)
    else if (channel == "sms") { var truncated = message[..160]; }

    // Conoce detalles de Slack (formato markdown)
    else if (channel == "slack") { return $"*{message}*"; }
}

// Cambiar la lógica de SMS requiere tocar el mismo archivo que maneja emails
```

### 🎯 Señales de Advertencia

- ✖️ Cadena if-else que verifica un parámetro de "tipo" o "canal"
- ✖️ Bloque `else` que lanza `NotSupportedException`
- ✖️ Un método que conoce detalles de TODAS las implementaciones
- ✖️ Imports de múltiples bibliotecas (SMTP, HTTP, SDKs) en una clase
- ✖️ El método crece con cada nueva integración

### 📖 Ver la Solución

Revisa `../../Solution/Notifications/` para ver el **patrón Strategy con colecciones**:

- `INotificationChannel.cs` - **Abstracción** cerrada para modificación
- `EmailNotificationChannel.cs` - **Implementación** para email
- `SmsNotificationChannel.cs` - **Implementación** para SMS
- `SlackNotificationChannel.cs` - **Implementación** para Slack
- `NotificationService.cs` - **Orquestador** que recibe múltiples canales inyectados

**Agregar Teams**: Crea `TeamsNotificationChannel.cs`, registra en DI — ¡CERO modificaciones a código existente!
