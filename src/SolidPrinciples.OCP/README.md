# Principio Abierto/Cerrado (OCP)

> "Las entidades de software (clases, módulos, funciones) deben estar abiertas para extensión,
> pero cerradas para modificación."
> — Bertrand Meyer

## 🎯 Objetivos de aprendizaje

Después de estudiar este módulo, podrás:

1. Identificar código que DEBE SER MODIFICADO cada vez que se necesita un nuevo comportamiento (cadenas switch/if-else en cadenas de "tipo")
2. Aplicar el **patrón Estrategia** para hacer una clase extensible sin cambiar su fuente
3. Usar **inyección de dependencias** para conectar nuevas estrategias en tiempo de ejecución
4. Escribir **componentes independientemente testables** que no compartan una única clase monolítica

## 📚 Artículo de referencia

Explicación completa con diagramas y refactorización paso a paso:
[Principio Abierto/Cerrado – Principios SOLID](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/open-closed)

## 🏗️ Contexto del dominio

Usamos el dominio **Gathering** (gestión de comunidades de práctica):

| Escenario                      | Problema                                                                                                               | Solución                                                                                              |
| ------------------------------ | ---------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------- |
| **Exportación de sesión**      | `SessionExporter` con un `switch` en formato (json/csv/xml) — agregar PDF significa editar la clase                    | Interfaz `ISessionExportStrategy` + implementaciones por formato inyectadas en `SessionExportService` |
| **Notificaciones**             | `NotificationDispatcher` con una cadena `if-else` en canal (email/sms/slack) — agregar Teams significa editar la clase | Interfaz `INotificationChannel` + implementaciones por canal inyectadas en `NotificationService`      |
| **Almacenamiento de imágenes** | `ImageUploadService` con un `switch` en proveedor (local/azure) — agregar AWS S3 significa editar la clase             | Interfaz `IImageStore` + implementaciones por proveedor inyectadas en `ImageUploadService`            |

## � Casos de Uso Incluidos

Este módulo contiene **tres casos de uso** que demuestran violaciones y correcciones de OCP:

### 📁 Caso de Uso 1: Exportación de Sesiones

Demuestra cómo el patrón Strategy elimina switch/case statements para formatos.

| Carpeta                                              | Descripción                                                               |
| ---------------------------------------------------- | ------------------------------------------------------------------------- |
| [`Problem/SessionExport/`](Problem/SessionExport/)   | `SessionExporter` - Switch en formato que requiere modificación constante |
| [`Solution/SessionExport/`](Solution/SessionExport/) | Strategy pattern con ISessionExportStrategy (cerrado para modificación)   |

**Ver detalles**: [Problem/SessionExport/README.md](Problem/SessionExport/README.md) • [Solution/SessionExport/README.md](Solution/SessionExport/README.md)

### 📁 Caso de Uso 2: Canales de Notificación

Demuestra cómo el patrón Strategy elimina cadenas if-else para tipos de canales.

| Carpeta                                              | Descripción                                                        |
| ---------------------------------------------------- | ------------------------------------------------------------------ |
| [`Problem/Notifications/`](Problem/Notifications/)   | `NotificationDispatcher` - Cadena if-else que crece con cada canal |
| [`Solution/Notifications/`](Solution/Notifications/) | INotificationChannel + múltiples implementaciones inyectadas       |

**Ver detalles**: [Problem/Notifications/README.md](Problem/Notifications/README.md) • [Solution/Notifications/README.md](Solution/Notifications/README.md)

### 📁 Caso de Uso 3: Almacenamiento de Imágenes

Demuestra cómo el patrón Strategy elimina switch statements para proveedores de almacenamiento (filesystem, cloud).

| Carpeta                                            | Descripción                                                            |
| -------------------------------------------------- | ---------------------------------------------------------------------- |
| [`Problem/ImageStorage/`](Problem/ImageStorage/)   | `ImageUploadService` - Switch en proveedor que crece con cada storage  |
| [`Solution/ImageStorage/`](Solution/ImageStorage/) | IImageStore + implementaciones por proveedor (Local, Azure, AWS, etc.) |

**Ver detalles**: [Problem/ImageStorage/README.md](Problem/ImageStorage/README.md) • [Solution/ImageStorage/README.md](Solution/ImageStorage/README.md)

## ▶️ Ejecutar pruebas

```bash
dotnet test --filter "FullyQualifiedName~OCP"

```
