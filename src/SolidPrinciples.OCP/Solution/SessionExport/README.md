# Solución: Exportación de Sesiones con OCP

## La Solución: Patrón Strategy

El código de exportación ahora sigue OCP usando el **patrón Strategy**: cada formato es una clase separada que implementa una interfaz común.

### 1️⃣ `ISessionExportStrategy.cs` - Abstracción

**Responsabilidad**: Definir el contrato para exportar sesiones.

```csharp
// Esta interfaz NO cambia cuando agregas nuevos formatos
public interface ISessionExportStrategy
{
    string Export(SessionExportData session);
}
```

**Cerrada para modificación**: Agregar PDF, Markdown o YAML no requiere cambiar esta interfaz.

### 2️⃣ Estrategias Concretas - Implementaciones

Cada formato vive en su propia clase:

```csharp
// JsonSessionExporter: Solo cambia si cambia la lógica JSON
public sealed class JsonSessionExporter : ISessionExportStrategy
{
    public string Export(SessionExportData session) =>
        JsonSerializer.Serialize(session, ...);
}

// CsvSessionExporter: Solo cambia si cambia la lógica CSV
public sealed class CsvSessionExporter : ISessionExportStrategy
{
    public string Export(SessionExportData session) =>
        $"{session.Id},{session.Title},...";
}

// XmlSessionExporter: Solo cambia si cambia la lógica XML
public sealed class XmlSessionExporter : ISessionExportStrategy
{
    public string Export(SessionExportData session) =>
        $"<session>...</session>";
}
```

**Abierto para extensión**: Agregar un nuevo formato es agregar una nueva clase.

### 3️⃣ `SessionExportService.cs` - Orquestador

**Responsabilidad**: Delegar export a la estrategia apropiada.

```csharp
// Este servicio NO cambia cuando agregas nuevos formatos
public sealed class SessionExportService
{
    private readonly ISessionExportStrategy _strategy;

    public SessionExportService(ISessionExportStrategy strategy)
    {
        _strategy = strategy;
    }

    public string Export(SessionExportData session) =>
        _strategy.Export(session);
}
```

## 🎯 Beneficios Alcanzados

### Cerrado para Modificación

Clases existentes NO se modifican cuando agregas formatos:

```csharp
// Agregar PDF: Crea PdfSessionExporter.cs (archivo nuevo)
public sealed class PdfSessionExporter : ISessionExportStrategy
{
    public string Export(SessionExportData session) => /* lógica PDF */;
}

// Registra en DI — sin modificar código existente
services.AddScoped<ISessionExportStrategy, PdfSessionExporter>();

// Agregar Markdown: Crea MarkdownSessionExporter.cs (archivo nuevo)
public sealed class MarkdownSessionExporter : ISessionExportStrategy
{
    public string Export(SessionExportData session) => /* lógica Markdown */;
}

// CERO cambios a:
// - ISessionExportStrategy.cs
// - JsonSessionExporter.cs
// - CsvSessionExporter.cs
// - XmlSessionExporter.cs
// - SessionExportService.cs
```

### Abierto para Extensión

Extiendes funcionalidad agregando nuevas clases, no modificando existentes:

```csharp
// Historial de commits con OCP:
// commit 1: "Agregado JsonSessionExporter.cs"      → Archivo nuevo
// commit 2: "Agregado CsvSessionExporter.cs"       → Archivo nuevo
// commit 3: "Agregado XmlSessionExporter.cs"       → Archivo nuevo
// commit 4: "Fixed bug en JSON"                    → Modifica solo JsonSessionExporter.cs
// commit 5: "Agregado PdfSessionExporter.cs"       → Archivo nuevo
// commit 6: (No hay commit 6, PDF no rompió CSV)   → Sin side effects
```

### Testabilidad Mejorada

Cada formato se prueba de forma aislada:

```csharp
[Fact]
public void JsonExporter_Should_Serialize_Session()
{
    // Prueba SOLO la lógica JSON, no CSV ni XML
    var exporter = new JsonSessionExporter();
    var result = exporter.Export(sessionData);

    result.Should().Contain("\"title\":");
}

[Fact]
public void CsvExporter_Should_Format_As_Csv()
{
    // Prueba SOLO la lógica CSV
    var exporter = new CsvSessionExporter();
    var result = exporter.Export(sessionData);

    result.Should().Contain(",");
}
```

## ⚙️ Configuración de Inyección de Dependencias

Para que `SessionExportService` reciba automáticamente todas las implementaciones de `ISessionExportStrategy`, debes **registrarlas en el contenedor de DI**:

### Program.cs (ASP.NET Core)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Registra TODAS las implementaciones de la interfaz
builder.Services.AddScoped<ISessionExportStrategy, JsonSessionExporter>();
builder.Services.AddScoped<ISessionExportStrategy, CsvSessionExporter>();
builder.Services.AddScoped<ISessionExportStrategy, XmlSessionExporter>();

// Registra el servicio que las consume
builder.Services.AddScoped<SessionExportService>();

var app = builder.Build();
```

### ¿Cómo Funciona?

Cuando el contenedor de DI resuelve `SessionExportService`:

1. Ve que el constructor requiere `IEnumerable<ISessionExportStrategy>`
2. Busca **TODAS** las implementaciones registradas de esa interfaz
3. Crea una instancia de cada una: `JsonSessionExporter`, `CsvSessionExporter`, `XmlSessionExporter`
4. Las empaqueta en una colección `IEnumerable<ISessionExportStrategy>`
5. Las inyecta en el constructor de `SessionExportService`

### Agregar un Nuevo Formato

Para agregar soporte PDF, solo necesitas:

```csharp
// PASO 1: Crear PdfSessionExporter.cs (archivo nuevo)
public sealed class PdfSessionExporter : ISessionExportStrategy
{
    public string Format => "pdf";
    public string Export(SessionExportData session) => /* lógica PDF */;
    public string ExportAll(IReadOnlyList<SessionExportData> sessions) => /* lógica PDF */;
}

// PASO 2: Registrar en Program.cs (solo UNA línea)
builder.Services.AddScoped<ISessionExportStrategy, PdfSessionExporter>();

```

## 🔗 Patrones Aplicados

- **Strategy Pattern**: `ISessionExportStrategy` + implementaciones concretas
- **Dependency Injection**: `SessionExportService` recibe estrategias vía constructor
- **Service Locator Pattern**: Selecciona estrategia por nombre de formato
- **Open/Closed Principle**: Extiendes sin modificar
