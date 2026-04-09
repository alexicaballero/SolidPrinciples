# Principio de Responsabilidad Única (SRP)

> **Una clase debe tener una, y solo una, razón para cambiar.**
> — Robert C. Martin

El Principio de Responsabilidad Única establece que cada clase debe encapsular una sola parte de la funcionalidad proporcionada por el sistema. Cuando una clase tiene más de una responsabilidad, los cambios en una responsabilidad pueden romper la otra. Esto conduce a código frágil, difícil de probar y que resiste el cambio.

## Por qué importa

En proyectos del mundo real, las violaciones de SRP crean **clases Dios** — clases grandes que saben demasiado y hacen demasiado. Cuando cambia la plantilla de correo electrónico, modificas la misma clase que maneja el acceso a la base de datos. Cuando cambia el formato del registro, arriesgas romper la lógica comercial. El resultado: cada cambio es riesgoso, las pruebas son lentas y frágiles, y la base de código se convierte en un campo minado.

## Objetivos de aprendizaje

Después de estudiar este módulo, deberías poder:

- Identificar clases con múltiples responsabilidades (clases Dios)
- Reconocer señales de advertencia: nombres de clase vagos, importaciones mixtas, métodos con "and" en el nombre
- Extraer clases enfocadas donde cada una tiene una única razón para cambiar
- Probar cada responsabilidad de forma aislada sin dependencias de infraestructura
- Aplicar el patrón Command Handler para separar orquestación de lógica comercial

## Casos de Uso Incluidos

Este módulo contiene **dos casos de uso** que demuestran violaciones y correcciones de SRP:

### 📁 Caso de Uso 1: Creación de Sesiones

Demuestra cómo dividir una clase Dios que maneja validación, persistencia, notificaciones y logging.

| Carpeta                                                  | Descripción                                               |
| -------------------------------------------------------- | --------------------------------------------------------- |
| [`Problem/SessionCreation/`](Problem/SessionCreation/)   | `SessionManager.cs` - Clase Dios con 5 responsabilidades  |
| [`Solution/SessionCreation/`](Solution/SessionCreation/) | Command Handler + Repository + Notificaciones (separados) |

**Ver detalles**: [Problem/SessionCreation/README.md](Problem/SessionCreation/README.md) • [Solution/SessionCreation/README.md](Solution/SessionCreation/README.md)

### 📁 Caso de Uso 2: Generación de Reportes

Demuestra cómo aplicar el patrón Facade separando recuperación de datos, formateo y entrega.

| Carpeta                                                    | Descripción                                                     |
| ---------------------------------------------------------- | --------------------------------------------------------------- |
| [`Problem/ReportGeneration/`](Problem/ReportGeneration/)   | `CommunityReportService.cs` - Datos + formateo + entrega juntos |
| [`Solution/ReportGeneration/`](Solution/ReportGeneration/) | DataProvider + Formatter + Sender (Strategy pattern)            |

**Ver detalles**: [Problem/ReportGeneration/README.md](Problem/ReportGeneration/README.md) • [Solution/ReportGeneration/README.md](Solution/ReportGeneration/README.md)

## Referencia del artículo

📖 [Principio de Responsabilidad Única](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/single-responsibility)

## Cómo ejecutar las pruebas

```bash
dotnet test --filter "FullyQualifiedName~SRP"

```
