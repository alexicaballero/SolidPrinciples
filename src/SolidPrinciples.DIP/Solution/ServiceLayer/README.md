# Solución: Service Layer con Dependencias Invertidas

## La Solución: Abstracciones Inyectadas

El servicio ahora depende de **interfaces** inyectadas vía constructor, eliminando el acoplamiento a EF Core y FileSystem.

### ANTES: Dependencias Directas

```
CommunityService (Application)
    ↓ depende de ↓
EFCoreCommunityRepository (Infrastructure)
FileSystemLogger (Infrastructure)
```

### DESPUÉS: Dependencias Invertidas

```
CommunityService (Application)
    ↓ depende de ↓
ICommunityRepository (Application) ✓
ILogger<CommunityService> (abstracción estándar) ✓
    ↑ implementado por ↑
EFCoreCommunityRepository (Infrastructure)
FileLogger / ConsoleLogger / AzureLogger (Infrastructure)
```

### Abstracciones

```csharp
// Abstracción de persistencia en capa Application
public interface ICommunityRepository
{
    Task AddAsync(Community community);
    Task<Community?> GetByIdAsync(Guid id);
    Task<IEnumerable<Community>> GetAllAsync();
    Task UpdateAsync(Community community);
}

// Logging usa abstracción estándar de Microsoft
// ILogger<T> ya está en Microsoft.Extensions.Logging.Abstractions
```

### Service con Dependencias Inyectadas

```csharp
// Servicio depende de abstracciones, NO de EF Core ni FileSystem
public sealed class CommunityService
{
    private readonly ICommunityRepository _repository;
    private readonly ILogger<CommunityService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección vía constructor
    public CommunityService(
        ICommunityRepository repository,
        ILogger<CommunityService> logger,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Community>> CreateCommunity(
        string name,
        string description,
        Guid adminId)
    {
        // NO hay `new DbContext()` — usa dependencias inyectadas
        _logger.LogInformation(
            "Creating community {Name} for admin {AdminId}",
            name, adminId);

        var community = Community.Create(name, description, adminId);
        if (community.IsFailure)
            return Result.Failure<Community>(community.Error);

        await _repository.AddAsync(community.Value);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
            "Community {CommunityId} created successfully",
            community.Value.Id);

        return community;
    }
}
```

## 🎯 Beneficios Alcanzados

### Testeable en Unidad

```csharp
[Fact]
public async Task Should_Create_Community()
{
    // Mocks en lugar de EF Core y FileSystem
    var repository = Substitute.For<ICommunityRepository>();
    var logger = Substitute.For<ILogger<CommunityService>>();
    var unitOfWork = Substitute.For<IUnitOfWork>();

    var service = new CommunityService(repository, logger, unitOfWork);

    var result = await service.CreateCommunity("SOLID Meetup", "...", adminId);

    // Sin SQL, sin archivos — prueba unitaria pura
    result.IsSuccess.Should().BeTrue();
    await repository.Received(1).AddAsync(Arg.Any<Community>());
}
```

### Implementaciones Intercambiables

```csharp
// Producción: EF Core + Azure Application Insights
services.AddScoped<ICommunityRepository, EFCoreCommunityRepository>();
services.AddApplicationInsightsTelemetry();

// Desarrollo: In-memory + Console
services.AddScoped<ICommunityRepository, InMemoryCommunityRepository>();
services.AddLogging(builder => builder.AddConsole());

// Integración: Dapper + File
services.AddScoped<ICommunityRepository, DapperCommunityRepository>();
services.AddLogging(builder => builder.AddFile("logs/app.log"));

// CERO cambios al servicio
```

### Sin Acoplamiento a ORM

```csharp
// Servicio NO conoce EF Core
namespace Gathering.Application;

using Microsoft.Extensions.Logging;  // ✓ Abstracción estándar
// NO importa Microsoft.EntityFrameworkCore ✓

public sealed class CommunityService
{
    private readonly ICommunityRepository _repository; // ✓ Interfaz local
    // ...
}
```

### Configuración Externalizada

```csharp
// Connection strings en appsettings.json, no hardcoded
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=Gathering;..."
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}

// Configuración en Startup
services.AddDbContext<GatheringDbContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("Default")));

services.AddScoped<ICommunityRepository, EFCoreCommunityRepository>();
```

### Diferentes Implementaciones de Repository

```csharp
// EF Core
public sealed class EFCoreCommunityRepository : ICommunityRepository
{
    private readonly GatheringDbContext _context;

    public EFCoreCommunityRepository(GatheringDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Community community)
    {
        await _context.Communities.AddAsync(community);
    }
}

// Dapper (más ligero)
public sealed class DapperCommunityRepository : ICommunityRepository
{
    private readonly IDbConnection _connection;

    public async Task AddAsync(Community community)
    {
        await _connection.ExecuteAsync(
            "INSERT INTO Communities (Id, Name, ...) VALUES (@Id, @Name, ...)",
            community);
    }
}

// In-memory (testing)
public sealed class InMemoryCommunityRepository : ICommunityRepository
{
    private readonly List<Community> _communities = new();

    public Task AddAsync(Community community)
    {
        _communities.Add(community);
        return Task.CompletedTask;
    }
}
```

### Logging Flexible

```csharp
// Console para desarrollo
builder.Services.AddLogging(config => config.AddConsole());

// Azure Application Insights para producción
builder.Services.AddApplicationInsightsTelemetry();

// Serilog estructurado
builder.Host.UseSerilog((context, config) =>
    config.WriteTo.Console()
          .WriteTo.File("logs/app.log")
          .WriteTo.AzureAnalytics(...));

// Todo sin modificar CommunityService
```

## 🔗 Patrones Aplicados

- **Dependency Inversion Principle**: Service depende de abstracciones
- **Dependency Injection**: Dependencias inyectadas vía constructor
- **Repository Pattern**: `ICommunityRepository` abstrae persistencia
- **Unit of Work Pattern**: Manejo transaccional
- **Clean Architecture**: Application no depende de Infrastructure
- **Configuration Externalization**: Settings fuera del código
