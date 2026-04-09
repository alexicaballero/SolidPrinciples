# Caso de Uso 2: Service Layer con Dependencias Concretas

## El Problema: `new` en Capa de Servicio

La clase `CommunityService` viola DIP porque **instancia directamente** repositorios y loggers concretos.

### 🚩 Dependencias Hardcodeadas

```csharp
// Servicio de alto nivel depende de implementaciones concretas
public sealed class CommunityService
{
    public void CreateCommunity(string name, string description, Guid adminId)
    {
        // VIOLA DIP: Instancia EF Core repository con new
        var repository = new EFCoreCommunityRepository(
            new DbContextOptionsBuilder<GatheringDbContext>()
                .UseSqlServer("Server=localhost;...")
                .Options);

        // VIOLA DIP: Instancia FileSystemLogger con new
        var logger = new FileSystemLogger("C:\\logs\\app.log");

        // Lógica de negocio...
        var community = new Community(name, description, adminId);
        repository.Add(community);
        logger.Log($"Community created: {community.Id}");
    }
}
```

### 💥 Problemas de este Diseño

**Acoplamiento a EF Core**:

- Service conoce `EFCoreCommunityRepository` concreta
- Service conoce `DbContextOptionsBuilder` de EF Core
- Imposible cambiar a Dapper, ADO.NET sin modificar Service

**Acoplamiento al sistema de archivos**:

- Service conoce `FileSystemLogger` concreta
- Hardcodea ruta de archivo
- Imposible cambiar a consola, Application Insights, Azure Monitor

**NO testeable**:

```csharp
[Fact]
public void Should_Create_Community()
{
    var service = new CommunityService();

    // NO puedes ejecutar esto sin:
    // - SQL Server con esquema correcto
    // - Permisos de escritura en C:\logs\

    service.CreateCommunity("SOLID Meetup", "...", adminId);
}
```

**Configuración hardcodeada**:

```csharp
// Connection string directamente en código
"Server=localhost;Database=Gathering;..."

// Ruta de log hardcoded
"C:\\logs\\app.log"

// Imposible cambiar por configuración externa
```

### 🎯 Señales de Advertencia

- ✖️ `new DbContext()`, `new SqlConnection()` en servicios
- ✖️ Imports de `Microsoft.EntityFrameworkCore`
- ✖️ Imports de `System.IO.File`, `System.Console`
- ✖️ Connection strings/rutas hardcodeadas
- ✖️ Servicios que crean sus propias dependencias

### 📖 Ver la Solución

Revisa `../../Solution/ServiceLayer/` para ver **inversión de dependencias**:

- `CommunityService` depende de abstracciones (`ICommunityRepository`, `ILogger`)
- Dependencias inyectadas vía constructor
- Testeable con mocks/in-memory
- Configuración externalizada
- Service NO conoce EF Core, FileSystem
