# Principio de Inversión de Dependencias (DIP)

> "Los módulos de alto nivel no deben depender de módulos de bajo nivel. Ambos deben depender de abstracciones.
> Las abstracciones no deben depender de detalles. Los detalles deben depender de abstracciones."
> — Robert C. Martin

## 🎯 Objetivos de Aprendizaje

Después de estudiar este módulo podrás:

1. Identificar **acoplamiento estrecho** entre lógica empresarial de alto nivel e infraestructura de bajo nivel
2. Reconocer código que **instancia dependencias con `new`** en lugar de recibirlas
3. Aplicar **inyección de dependencias** para invertir el flujo de control
4. Diseñar **abstracciones (interfaces)** que desacopen la lógica empresarial de la infraestructura
5. Escribir **código testeable en unidad** inyectando dependencias simuladas/en memoria

## 📚 Artículo de Referencia

Explicación completa con diagramas y refactorización paso a paso:
[Principio de Inversión de Dependencias – Principios SOLID](https://calm-field-0d87ced10.6.azurestaticapps.net/post/en/solid-principles/dependency-inversion)

## 🏗️ Contexto de Dominio

Usamos el dominio **Gathering** (gestión de Comunidades de Práctica):

| Escenario                  | Problema                                                                                                                                                                           | Solución                                                                                                                                                                        |
| -------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Controlador de Comando** | `CreateSessionHandler` instancia directamente `SqlSessionRepository` y `EmailNotificationSender` con `new` — no se puede hacer pruebas unitarias sin base de datos y servidor SMTP | `CreateSessionHandler` depende de abstracciones `ISessionRepository` e `INotificationSender`, inyectadas vía constructor — testeable con implementaciones en memoria/simuladas  |
| **Capa de Servicio**       | `CommunityService` instancia directamente `EFCoreCommunityRepository` y `FileSystemLogger` — estrechamente acoplado a EF Core y sistema de archivos                                | `CommunityService` depende de abstracciones `ICommunityRepository` e `ILogger`, inyectadas vía constructor — implementaciones intercambiables (EF Core, Dapper, Consola, Azure) |

## � Casos de Uso Incluidos

Este módulo contiene **dos casos de uso** que demuestran violaciones y correcciones de DIP:

### 📁 Caso de Uso 1: Command Handler

Demuestra cómo eliminar instanciación directa (`new`) de dependencias en handlers.

| Carpeta                                                | Descripción                                                             |
| ------------------------------------------------------ | ----------------------------------------------------------------------- |
| [`Problem/CommandHandler/`](Problem/CommandHandler/)   | Handler usa `new` para SqlSessionRepository y EmailNotificationSender   |
| [`Solution/CommandHandler/`](Solution/CommandHandler/) | Handler depende de ISessionRepository y INotificationSender (inyección) |

**Ver detalles**: [Problem/CommandHandler/README.md](Problem/CommandHandler/README.md) • [Solution/CommandHandler/README.md](Solution/CommandHandler/README.md)

### 📁 Caso de Uso 2: Service Layer

Demuestra cómo eliminar acoplamiento a EF Core y FileSystem en servicios.

| Carpeta                                            | Descripción                                                         |
| -------------------------------------------------- | ------------------------------------------------------------------- |
| [`Problem/ServiceLayer/`](Problem/ServiceLayer/)   | Service usa `new` para EFCoreCommunityRepository y FileSystemLogger |
| [`Solution/ServiceLayer/`](Solution/ServiceLayer/) | Service depende de ICommunityRepository y ILogger (inyección)       |

**Ver detalles**: [Problem/ServiceLayer/README.md](Problem/ServiceLayer/README.md) • [Solution/ServiceLayer/README.md](Solution/ServiceLayer/README.md)

## ⚠️ Señales de Alerta de Violaciones de DIP

Busca estas malas prácticas de codificación:

1. **Palabra clave `new` en lógica empresarial**: Código de alto nivel instancia dependencias de bajo nivel
2. **Infraestructura hardcodeada**: Código importa `System.Data.SqlClient`, `System.Net.Mail`, `MongoDB.Driver` directamente
3. **Código no testeable**: Pruebas unitarias requieren base de datos, servidor SMTP, sistema de archivos
4. **Constructores sin parámetros**: Constructores sin parámetros que llaman a `new` internamente
5. **Dependencias estáticas**: `SqlSessionRepository.Instance`, `Logger.Current`
6. **Violaciones de importación**: Capa de Aplicación importando capa de Infraestructura

## Beneficios de la Inversión de Dependencias

Cuando inviertes dependencias correctamente:

- **Testabilidad**: Inyecta implementaciones en memoria/simuladas para pruebas unitarias
- **Flexibilidad**: Intercambia implementaciones en tiempo de ejecución (SQL → MongoDB, Email → Slack)
- **Desacoplamiento**: La lógica empresarial no conoce detalles de infraestructura
- **Arquitectura en capas**: La política de alto nivel (Aplicación) no depende de detalles de bajo nivel (Infraestructura)
- **Evolución más fácil**: Agrega nuevas implementaciones sin modificar código existente

## 🔗 Relación con Otros Principios

- **OCP**: DIP habilita extensibilidad — nuevas implementaciones pueden agregarse sin modificar código de alto nivel
- **ISP**: Las abstracciones deben ser enfocadas (segregadas) para que las implementaciones no se vean forzadas a depender de métodos que no usan
- **SRP**: La inyección de dependencias separa la construcción de objetos de la lógica empresarial
- **LSP**: Todas las implementaciones de una abstracción deben ser substituibles

## 🎯 La "Inversión" Explicada

**Flujo de dependencia tradicional** (viola DIP):

```
CreateSessionHandler (alto nivel)
    ↓ depende de ↓
SqlSessionRepository (bajo nivel)
```

El módulo de alto nivel depende del módulo de bajo nivel. Cambiar la implementación de SQL requiere modificar el controlador.

**Flujo de dependencia invertido** (sigue DIP):

```
CreateSessionHandler (alto nivel)
    ↓ depende de ↓
ISessionRepository (abstracción)
    ↑ implementado por ↑
SqlSessionRepository (bajo nivel)
```

Tanto el módulo de alto nivel como el de bajo nivel dependen de la abstracción. La flecha de dependencia está invertida: el bajo nivel depende de la interfaz del alto nivel.

## 🛠️ Contenedor de Inyección de Dependencias

En ASP.NET Core, registra dependencias en `Program.cs`:

```csharp
// Registra abstracciones e implementaciones
builder.Services.AddScoped<ISessionRepository, SqlSessionRepository>();
builder.Services.AddScoped<INotificationSender, EmailNotificationSender>();

// El contenedor DI inyecta automáticamente dependencias en constructores
// Ejemplo: CreateSessionHandler constructor recibe ISessionRepository e INotificationSender
```

Para pruebas, inyecta dependencias manualmente:

```csharp
// Prueba unitaria: inyecta implementaciones en memoria
var repository = new InMemorySessionRepository();
var notificationSender = new ConsoleNotificationSender();
var handler = new CreateSessionHandler(repository, notificationSender);
```

## ▶️ Ejecutar Pruebas

```bash
dotnet test --filter "FullyQualifiedName~DIP"
```

## 📖 Fuentes de Referencia

- **Clean Architecture** por Robert C. Martin — capítulo Dependency Rule
- **Dependency Injection Principles, Practices, and Patterns** por Steven van Deursen y Mark Seemann
- [Proyecto de Referencia Gathering](https://github.com/alexicaballero/gathering/tree/main/backend) — Ver `Application/Abstractions` para interfaces de repositorio, `Infrastructure/Persistence` para implementaciones
- [Docs de Microsoft: Inyección de Dependencias en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
