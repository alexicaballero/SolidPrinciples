# OCP — Problema (Violación)

## ¿Qué está mal?

Ambas clases en esta carpeta violan el **Principio Abierto/Cerrado** porque deben ser **modificadas** cada vez que se necesita una nueva variante de comportamiento.

### SessionExporter

```csharp
switch (format.ToLowerInvariant())
{
    case "json": // ...
    case "csv":  // ...
    case "xml":  // ...
    default: throw new NotSupportedException(...);
}
```

**¿Quieres agregar exportación de PDF?** Debes:
1. Abrir `SessionExporter.cs`
2. Agregar un nuevo `case "pdf":` en **ambos** `Export` y `ExportAll`
3. Escribir la lógica en línea junto a json/csv/xml
4. Re-probar TODOS los formatos, porque comparten el mismo método
5. Riesgo de romper algo que ya funcionaba

### NotificationDispatcher

```csharp
if (channel == "email")      { ... }
else if (channel == "sms")   { ... }
else if (channel == "slack")  { ... }
else throw new NotSupportedException(...);
```

**¿Quieres agregar Microsoft Teams?** Mismo problema — edita el archivo, riesgo con canales existentes.

## 🔍 Señales de advertencia

| Olor | Dónde |
|------|-------|
| Switch/case en una cadena de "tipo" o "formato" | `SessionExporter.Export` |
| Cadena if-else verificando una cadena de "canal" | `NotificationDispatcher.Send` |
| `default: throw new NotSupportedException(...)` | Ambas clases |
| Lógica de formato duplicada en métodos | `Export` vs `ExportAll` |
| Todas las variantes acopladas en una clase | Ambas clases |

## 🧪 Dolor de pruebas

- Las pruebas para json, csv y xml pasan todas a través de **la misma clase** — un cambio en un formato fuerza re-ejecutar todas las pruebas
- Los formatos no soportados fallan en **tiempo de ejecución**, no en tiempo de compilación
- No puedes probar un nuevo formato sin modificar primero la clase de producción


