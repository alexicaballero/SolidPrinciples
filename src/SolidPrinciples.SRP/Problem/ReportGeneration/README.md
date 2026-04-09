# Caso de Uso 2: Generación de Reportes

## El Problema: Clase Dios CommunityReportService

La clase `CommunityReportService` viola SRP al manejar **3 responsabilidades diferentes**:

1. **Recuperación de datos** - Consulta la base de datos para obtener sesiones
2. **Formateo de reporte** - Construye salida HTML con StringBuilder
3. **Entrega de reporte** - Envía el resultado por correo electrónico

### 🚩 Señales de Advertencia

```csharp
// Método con "and" en el nombre = múltiples responsabilidades
public void GenerateAndSendReport(...)

// La clase conoce detalles de 3 capas diferentes
private string FetchSessionData(...)      // Data access
private void FormatAsHtml(...)            // Presentation
private void SendEmail(...)               // Infrastructure
```

### 💥 Razones para Cambiar

Esta clase debe modificarse cuando:

- Cambia la fuente de datos (SQL → API REST)
- Cambia el formato de salida (HTML → PDF, Excel, JSON)
- Cambia el método de entrega (email → Slack, webhook)
- Cambia el diseño HTML (nuevos estilos, estructura)
- Optimización de consultas SQL
- Agregar filtros o parámetros al reporte

### 🧪 Problemas de Prueba

```csharp
// No puedes probar el formateo sin la base de datos
[Fact]
public void Should_Format_Report_As_Html()
{
    // Necesitas una DB real porque FetchSessionData() está acoplado
    var service = new CommunityReportService(connectionString, smtpHost);
    // ¿Cómo pruebas solo la lógica de formateo HTML?
}

// No puedes probar diferentes formatos sin duplicar lógica de datos
// Si quieres agregar formato CSV, debes copiar y modificar esta clase
```

### 📖 Ver la Solución

Revisa `../../Solution/ReportGeneration/` para ver cómo SRP separa preocupaciones:

- `CommunityReportGenerator.cs` - **Solo** orquestación (patron Facade)
- `IReportDataProvider.cs` - **Solo** interfaz de acceso a datos
- `IReportFormatter.cs` - **Solo** interfaz de formateo
- `HtmlReportFormatter.cs` - **Solo** implementación HTML
- `IReportSender.cs` - **Solo** interfaz de entrega

Ahora puedes:

- Agregar formato PDF sin tocar el acceso a datos
- Cambiar la fuente de datos sin tocar el formateo
- Enviar a Slack sin tocar nada más
- Probar cada responsabilidad de forma aislada
