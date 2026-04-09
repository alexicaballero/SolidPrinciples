# Solución: Notificaciones con OCP

## La Solución: Patrón Strategy con Múltiples Implementaciones

El código de notificaciones ahora sigue OCP usando el **patrón Strategy**: cada canal es una clase separada que implementa una interfaz común.

### 1️⃣ `INotificationChannel.cs` - Abstracción

**Responsabilidad**: Definir el contrato para canales de notificación.

```csharp
// Esta interfaz NO cambia cuando agregas nuevos canales
public interface INotificationChannel
{
    string Name { get; }
    Task<string> SendAsync(string recipient, string message);
}
```

**Cerrada para modificación**: Agregar Teams, Push o Webhooks no requiere cambiar esta interfaz.

### 2️⃣ Canales Concretos - Implementaciones

Cada canal vive en su propia clase:

```csharp
// EmailNotificationChannel: Solo cambia si cambia la lógica SMTP
public sealed class EmailNotificationChannel : INotificationChannel
{
    public string Name => "email";
    public Task<string> SendAsync(string recipient, string message) =>
        Task.FromResult($"EMAIL a {recipient}: {message}");
}

// SmsNotificationChannel: Solo cambia si cambia la lógica SMS
public sealed class SmsNotificationChannel : INotificationChannel
{
    public string Name => "sms";
    public Task<string> SendAsync(string recipient, string message)
    {
        var truncated = message.Length > 160 ? message[..160] : message;
        return Task.FromResult($"SMS a {recipient}: {truncated}");
    }
}

// SlackNotificationChannel: Solo cambia si cambia la lógica Slack
public sealed class SlackNotificationChannel : INotificationChannel
{
    public string Name => "slack";
    public Task<string> SendAsync(string recipient, string message) =>
        Task.FromResult($"SLACK a #{recipient}: *{message}*");
}
```

**Abierto para extensión**: Agregar un nuevo canal es agregar una nueva clase.

### 3️⃣ `NotificationService.cs` - Orquestador

**Responsabilidad**: Gestionar múltiples canales inyectados.

```csharp
// Este servicio NO cambia cuando agregas nuevos canales
public sealed class NotificationService
{
    private readonly IEnumerable<INotificationChannel> _channels;

    public NotificationService(IEnumerable<INotificationChannel> channels)
    {
        _channels = channels;
    }

    public async Task<string> SendAsync(string channelName, string recipient, string message)
    {
        var channel = _channels.FirstOrDefault(c =>
            c.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase));

        if (channel == null)
            throw new NotSupportedException($"Canal '{channelName}' no encontrado.");

        return await channel.SendAsync(recipient, message);
    }

    // Broadcast a TODOS los canales registrados
    public async Task<List<string>> BroadcastAsync(string message,
        IReadOnlyList<(string Channel, string Recipient)> targets)
    {
        var results = new List<string>();
        foreach (var (channelName, recipient) in targets)
        {
            results.Add(await SendAsync(channelName, recipient, message));
        }
        return results;
    }
}
```

## 🎯 Beneficios Alcanzados

### Cerrado para Modificación

Clases existentes NO se modifican cuando agregas canales:

```csharp
// Agregar Teams: Crea TeamsNotificationChannel.cs (archivo nuevo)
public sealed class TeamsNotificationChannel : INotificationChannel
{
    public string Name => "teams";
    public Task<string> SendAsync(string recipient, string message) =>
        Task.FromResult($"TEAMS a {recipient}: {message}");
}

// Registra en DI — todos los canales se descubren automáticamente
services.AddScoped<INotificationChannel, EmailNotificationChannel>();
services.AddScoped<INotificationChannel, SmsNotificationChannel>();
services.AddScoped<INotificationChannel, SlackNotificationChannel>();
services.AddScoped<INotificationChannel, TeamsNotificationChannel>(); // ← nuevo

// CERO cambios a:
// - INotificationChannel.cs
// - EmailNotificationChannel.cs
// - SmsNotificationChannel.cs
// - SlackNotificationChannel.cs
// - NotificationService.cs
```

### Abierto para Extensión

```csharp
// Agregar Webhook
public sealed class WebhookNotificationChannel : INotificationChannel
{
    private readonly HttpClient _httpClient;

    public WebhookNotificationChannel(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string Name => "webhook";

    public async Task<string> SendAsync(string recipient, string message)
    {
        await _httpClient.PostAsJsonAsync(recipient, new { message });
        return $"WEBHOOK enviado a {recipient}";
    }
}

// Sin modificar código existente
```

### Composición Flexible

```csharp
// Configuración 1: Solo email
services.AddScoped<INotificationChannel, EmailNotificationChannel>();

// Configuración 2: Email + SMS para producción
services.AddScoped<INotificationChannel, EmailNotificationChannel>();
services.AddScoped<INotificationChannel, SmsNotificationChannel>();

// Configuración 3: Todos los canales
services.AddScoped<INotificationChannel, EmailNotificationChannel>();
services.AddScoped<INotificationChannel, SmsNotificationChannel>();
services.AddScoped<INotificationChannel, SlackNotificationChannel>();
services.AddScoped<INotificationChannel, TeamsNotificationChannel>();
services.AddScoped<INotificationChannel, WebhookNotificationChannel>();
```

### Testabilidad Total

Cada canal se prueba de forma aislada:

```csharp
[Fact]
public async Task EmailChannel_Should_Send_Full_Message()
{
    var channel = new EmailNotificationChannel();
    var result = await channel.SendAsync("user@example.com", "Hello World");

    result.Should().Contain("EMAIL");
    result.Should().Contain("Hello World");
}

[Fact]
public async Task SmsChannel_Should_Truncate_Long_Messages()
{
    var channel = new SmsNotificationChannel();
    var longMessage = new string('x', 200);

    var result = await channel.SendAsync("+123", longMessage);

    result.Should().Contain("SMS");
    result.Should().NotContain(new string('x', 200)); // truncado
}
``

## 🔗 Patrones Aplicados

- **Strategy Pattern**: `INotificationChannel` + múltiples implementaciones
- **Dependency Injection**: `NotificationService` recibe colección de canales
- **Open/Closed Principle**: Extiendes sin modificar
- **Service Locator Pattern**: Selecciona canal por nombre
```
