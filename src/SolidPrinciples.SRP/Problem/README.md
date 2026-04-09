# Problema — Violaciones de SRP

## Qué está mal

Estas clases manejan **múltiples responsabilidades** que deberían estar separadas. `SessionManager` es una clase Dios que valida entrada, escribe en la base de datos, envía correos electrónicos y registra en archivos — todo en un solo lugar. `CommunityReportService` obtiene datos, los formatea en HTML/CSV y los entrega por correo electrónico — tres preocupaciones distintas entrelazadas.

## Señales de Advertencia

- **Nombres de clase vagos**: "Manager" y "Service" son banderas rojas — no describen una responsabilidad específica
- **Importaciones mixtas**: Una clase que importa `System.Data`, `System.Net.Mail` Y `System.IO` está haciendo demasiado
- **Constructor inflado**: Necesitar una cadena de conexión, host SMTP Y ruta de archivo de registro para crear un objeto
- **Métodos con "and"**: `GenerateAndSendReport` — el nombre mismo revela múltiples responsabilidades
- **Patrones duplicados**: `CancelSession` repite el mismo patrón de SQL + correo electrónico + registro que `CreateSession`

## Impacto

Cuando cambian los requisitos:

- **¿Nuevo canal de notificación (Slack)?** → Debe modificar `SessionManager` y riesgo de romper la lógica de base de datos
- **¿Cambiar de SQL a una API?** → Debe modificar la misma clase que maneja el correo electrónico
- **¿Nuevo formato de informe (PDF)?** → Debe agregar un nuevo método a `CommunityReportService` y riesgo de romper la generación HTML
- **¿Cambiar formato de registro?** → Debe tocar la clase que maneja la orquestación comercial

Cada cambio conlleva el riesgo de romper la funcionalidad no relacionada.

## Archivos

| Archivo | Descripción |
|------|----------|
| `SessionManager.cs` | Clase Dios con 5 responsabilidades: validación, persistencia SQL, notificación de correo electrónico, registro de archivos, orquestación |
| `CommunityReportService.cs` | Tres responsabilidades en una clase: obtención de datos, formateo de informe, entrega de correo electrónico |


