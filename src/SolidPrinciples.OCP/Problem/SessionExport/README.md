# Caso de Uso 1: Exportación de Sesiones

## El Problema: Switch/Case en Formato

La clase `SessionExporter` viola OCP porque **debe ser modificada** cada vez que necesitas agregar un nuevo formato de exportación.

### 🚩 El Anti-Patrón: Switch en Tipo

```csharp
public string Export(SessionExportData session, string format)
{
    switch (format.ToLowerInvariant())
    {
        case "json": return /* lógica JSON */;
        case "csv":  return /* lógica CSV */;
        case "xml":  return /* lógica XML */;

        // ¿Necesitas PDF? Debes modificar ESTA clase
        // ¿Necesitas Markdown? Debes modificar ESTA clase
        // ¿Necesitas YAML? Debes modificar ESTA clase
        default: throw new NotSupportedException();
    }
}
```

### 💥 Por Qué Esto Viola OCP

**NO está cerrada para modificación**:

- Agregar formato PDF → Editar `SessionExporter.cs`
- Agregar formato Markdown → Editar `SessionExporter.cs`
- Agregar formato Excel → Editar `SessionExporter.cs`

**NO está abierta para extensión**:

- No puedes agregar un nuevo formato sin tocar código funcionando
- Cada cambio requiere re-probar TODOS los formatos existentes
- Riesgo de romper JSON mientras agregas PDF

### 🧪 Pesadilla de Mantenimiento

```csharp
// Historial de commits:
// commit 1: "Agregado soporte JSON"       → Edita SessionExporter
// commit 2: "Agregado soporte CSV"        → Edita SessionExporter
// commit 3: "Agregado soporte XML"        → Edita SessionExporter
// commit 4: "Fixed bug en JSON"           → Edita SessionExporter
// commit 5: "Agregado soporte PDF"        → Edita SessionExporter
// commit 6: "Fixed CSV roto por cambio PDF" → Edita SessionExporter
//
// CADA formato toca la MISMA clase → fragil, riesgoso, difícil de revisar
```

### 🎯 Señales de Advertencia

- ✖️ Switch/case o cadena if-else en un parámetro de "tipo"
- ✖️ Caso `default` que lanza `NotSupportedException`
- ✖️ Todos los formatos mezclados en un solo archivo
- ✖️ El método crece indefinidamente con cada requisito nuevo
- ✖️ Commits que modifican la misma clase una y otra vez

### 📖 Ver la Solución

Revisa `../../Solution/SessionExport/` para ver el **patrón Strategy** aplicado:

- `ISessionExportStrategy.cs` - **Abstracción** cerrada para modificación
- `JsonSessionExporter.cs` - **Estrategia concreta** para JSON
- `CsvSessionExporter.cs` - **Estrategia concreta** para CSV
- `XmlSessionExporter.cs` - **Estrategia concreta** para XML
- `SessionExportService.cs` - **Orquestador** que usa estrategias inyectadas

**Agregar PDF**: Crea `PdfSessionExporter.cs`, registra en DI — ¡CERO modificaciones a código existente!
