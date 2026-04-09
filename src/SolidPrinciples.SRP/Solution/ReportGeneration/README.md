# Solución: Generación de Reportes con SRP

## La Solución: Patrón Facade con Responsabilidades Separadas

La responsabilidad de "generar reporte" se divide usando el **patrón Facade** con colaboradores especializados:

### 1️⃣ `CommunityReportGenerator.cs` - Orquestador (Facade)

**Responsabilidad única**: Coordinar las tres operaciones sin conocer sus detalles de implementación.

```csharp
// Solo cambia si cambia el flujo de generación de reportes
public sealed class CommunityReportGenerator
{
    private readonly IReportDataProvider _dataProvider;
    private readonly IReportFormatter _formatter;
    private readonly IReportSender _sender;

    public async Task GenerateAndSend(Guid communityId, string recipient)
    {
        // 1. Obtener datos (responsabilidad delegada)
        var data = await _dataProvider.GetSessionsAsync(communityId);

        // 2. Formatear (responsabilidad delegada)
        var content = _formatter.Format(data);

        // 3. Enviar (responsabilidad delegada)
        await _sender.SendAsync(recipient, "Reporte de Comunidad", content);
    }
}
```

**Razones para cambiar**:

- Cambios en los pasos del proceso de reportes
- Nuevos parámetros de filtrado

### 2️⃣ `IReportDataProvider.cs` - Acceso a Datos

**Responsabilidad única**: Definir cómo obtener datos para reportes.

```csharp
// Solo cambia si cambia el contrato de datos
public interface IReportDataProvider
{
    Task<IEnumerable<SessionReportData>> GetSessionsAsync(Guid communityId);
}

// Implementaciones posibles:
// - SqlReportDataProvider (SQL directo)
// - RepositoryReportDataProvider (usa repositorio existente)
// - ApiReportDataProvider (consume API externa)
```

### 3️⃣ `IReportFormatter.cs` + `HtmlReportFormatter.cs` - Formateo

**Responsabilidad única**: Formatear datos en un formato específico.

```csharp
// Solo cambia si cambia el contrato de formateo
public interface IReportFormatter
{
    string Format(IEnumerable<SessionReportData> data);
}

// HtmlReportFormatter solo cambia si cambia el diseño HTML
public sealed class HtmlReportFormatter : IReportFormatter
{
    public string Format(IEnumerable<SessionReportData> data)
    {
        var html = new StringBuilder();
        html.Append("<html><body>");
        // ... solo lógica de formateo HTML
        html.Append("</body></html>");
        return html.ToString();
    }
}
```

**Nuevos formatos sin tocar código existente**:

```csharp
// Agregar formato PDF
public sealed class PdfReportFormatter : IReportFormatter { }

// Agregar formato Excel
public sealed class ExcelReportFormatter : IReportFormatter { }

// Agregar formato JSON
public sealed class JsonReportFormatter : IReportFormatter { }
```

### 4️⃣ `IReportSender.cs` - Entrega

**Responsabilidad única**: Entregar el reporte formateado.

```csharp
// Solo cambia si cambia el contrato de entrega
public interface IReportSender
{
    Task SendAsync(string recipient, string subject, string content);
}

// Implementaciones posibles:
// - EmailReportSender (SMTP)
// - SlackReportSender (Webhook)
// - FileReportSender (guardar en disco)
// - BlobReportSender (Azure Storage)
```

## 🎯 Beneficios Alcanzados

### Testabilidad Total

```csharp
[Fact]
public void Should_Format_Sessions_As_Html()
{
    // Probar formateo sin base de datos
    var formatter = new HtmlReportFormatter();
    var sessions = new List<SessionReportData>
    {
        new("SOLID Principles", "Jane Doe", DateTime.Now, "Scheduled")
    };

    var html = formatter.Format(sessions);

    html.Should().Contain("<h1>Informe de Comunidad</h1>");
    html.Should().Contain("SOLID Principles");
}

[Fact]
public async Task Should_Orchestrate_Report_Generation()
{
    // Probar orquestación sin infraestructura real
    var dataProvider = Substitute.For<IReportDataProvider>();
    var formatter = Substitute.For<IReportFormatter>();
    var sender = Substitute.For<IReportSender>();

    var generator = new CommunityReportGenerator(dataProvider, formatter, sender);

    await generator.GenerateAndSend(communityId, "user@example.com");

    await sender.Received(1).SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
}
```

### Extensibilidad con Open/Closed

Agregar nuevas capacidades **sin modificar** código existente:

```csharp
// Nuevo formato sin tocar HtmlReportFormatter
services.AddScoped<IReportFormatter, PdfReportFormatter>();

// Nuevo canal de entrega sin tocar EmailReportSender
services.AddScoped<IReportSender, WebhookReportSender>();

// Nueva fuente de datos sin tocar SQL
services.AddScoped<IReportDataProvider, GraphApiDataProvider>();
```

### Composición Flexible

```csharp
// Diferentes combinaciones para diferentes escenarios
public void ConfigureServices(IServiceCollection services)
{
    // Reportes HTML por email
    services.AddScoped<IReportFormatter, HtmlReportFormatter>();
    services.AddScoped<IReportSender, EmailReportSender>();

    // O reportes PDF a blob storage
    services.AddScoped<IReportFormatter, PdfReportFormatter>();
    services.AddScoped<IReportSender, BlobStorageReportSender>();

    // O reportes JSON a webhook
    services.AddScoped<IReportFormatter, JsonReportFormatter>();
    services.AddScoped<IReportSender, WebhookReportSender>();
}
```

### Mantenibilidad

- Cambios en SQL → Solo tocas `SqlReportDataProvider`
- Cambios en diseño HTML → Solo tocas `HtmlReportFormatter`
- Cambios en configuración SMTP → Solo tocas `EmailReportSender`
- Nuevo formato → Crear nueva clase, no modificar existentes

## 🔗 Patrones Aplicados

- **Facade Pattern**: `CommunityReportGenerator` simplifica interacción con subsistemas
- **Strategy Pattern**: Diferentes implementaciones de `IReportFormatter`
- **Dependency Inversion**: Generator depende de abstracciones
- **Interface Segregation**: Interfaces pequeñas y enfocadas
