# Solución: Command Handler con Dependencias Invertidas

## La Solución: Inyección de Dependencias

El handler ahora depende de **abstracciones** inyectadas vía constructor, invirtiendo la relación de dependencia.

### ANTES: Dependencias Directas

```
CreateSessionHandler (Application)
    ↓ depende de ↓
SqlSessionRepository (Infrastructure)
EmailNotificationSender (Infrastructure)
```

### DESPUÉS: Dependencias Invertidas

```
CreateSessionHandler (Application)
    ↓ depende de ↓
ISessionRepository (Application) ✓
INotificationSender (Application) ✓
    ↑ implementado por ↑
SqlSessionRepository (Infrastructure)
EmailNotificationSender (Infrastructure)
```

### Abstracciones

```csharp
// Abstracciones en capa Application
public interface ISessionRepository
{
    Task AddAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
}

public interface INotificationSender
{
    Task SendAsync(string recipient, string subject, string message);
}
```

### Handler con Dependencias Inyectadas

```csharp
// Handler depende de abstracciones, NO de implementaciones
public sealed class CreateSessionHandler
{
    private readonly ISessionRepository _repository;
    private readonly INotificationSender _sender;
    private readonly IUnitOfWork _unitOfWork;

    // Inyección vía constructor
    public CreateSessionHandler(
        ISessionRepository repository,
        INotificationSender sender,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _sender = sender;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Session>> Handle(CreateSessionCommand command)
    {
        // NO hay `new` — usa dependencias inyectadas
        var session = Session.Create(
            command.Title,
            command.Speaker,
            command.Date,
            command.CommunityId);

        if (session.IsFailure)
            return Result.Failure<Session>(session.Error);

        await _repository.AddAsync(session.Value);
        await _unitOfWork.SaveChangesAsync();

        await _sender.SendAsync(
            command.AdminEmail,
            "Sesión Creada",
            $"Nueva sesión: {session.Value.Title}");

        return session;
    }
}
```

## 🎯 Beneficios Alcanzados

### Testeable en Unidad

```csharp
[Fact]
public async Task Should_Create_Session_And_Send_Notification()
{
    // Mocks en lugar de infraestructura real
    var repository = Substitute.For<ISessionRepository>();
    var sender = Substitute.For<INotificationSender>();
    var unitOfWork = Substitute.For<IUnitOfWork>();

    var handler = new CreateSessionHandler(repository, sender, unitOfWork);

    var result = await handler.Handle(command);

    // Sin SQL Server, sin SMTP — prueba unitaria pura
    result.IsSuccess.Should().BeTrue();
    await repository.Received(1).AddAsync(Arg.Any<Session>());
    await sender.Received(1).SendAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
}
```

### Implementaciones Intercambiables

```csharp
// Producción: SQL + Email
services.AddScoped<ISessionRepository, SqlSessionRepository>();
services.AddScoped<INotificationSender, EmailNotificationSender>();

// Desarrollo: In-memory + Console
services.AddScoped<ISessionRepository, InMemorySessionRepository>();
services.AddScoped<INotificationSender, ConsoleNotificationSender>();

// Testing: Mocks
services.AddScoped<ISessionRepository, FakeSessionRepository>();
services.AddScoped<INotificationSender, NoOpNotificationSender>();

// CERO cambios al handler — solo configuración DI
```

### Arquitectura en Capas Correcta

```csharp
// Application NO importa Infrastructure
namespace Gathering.Application;

using Gathering.Domain;  // ✓ OK — Domain es el núcleo
// NO importa Gathering.Infrastructure ✓

public sealed class CreateSessionHandler
{
    private readonly ISessionRepository _repository; // ✓ Interfaz en Application
    // ...
}
```

### Decoradores y Cross-Cutting Concerns

```csharp
// Agregar logging sin modificar handler
public sealed class LoggingSessionRepository : ISessionRepository
{
    private readonly ISessionRepository _inner;
    private readonly ILogger<LoggingSessionRepository> _logger;

    public async Task AddAsync(Session session)
    {
        _logger.LogInformation("Adding session {SessionId}", session.Id);
        await _inner.AddAsync(session);
        _logger.LogInformation("Session {SessionId} added", session.Id);
    }
}

// Registro simple con decorator pattern
services.AddScoped<ISessionRepository, SqlSessionRepository>();
services.Decorate<ISessionRepository, LoggingSessionRepository>();

// CERO cambios al handler
```

### Evolutivo

```csharp
// Cambiar a MongoDB
services.AddScoped<ISessionRepository, MongoSessionRepository>();

// Cambiar a Slack en lugar de email
services.AddScoped<INotificationSender, SlackNotificationSender>();

// Agregar caché
services.Decorate<ISessionRepository, CachedSessionRepository>();

// Todo sin modificar CreateSessionHandler
```

## 🔗 Patrones Aplicados

- **Dependency Inversion Principle**: Handler depende de abstracciones
- **Dependency Injection**: Dependencias inyectadas vía constructor
- **Repository Pattern**: `ISessionRepository` abstrae persistencia
- **Unit of Work Pattern**: `IUnitOfWork` maneja transacciones
- **Decorator Pattern**: Agregar capacidades sin modificar handler
- **Clean Architecture**: Application no depende de Infrastructure
