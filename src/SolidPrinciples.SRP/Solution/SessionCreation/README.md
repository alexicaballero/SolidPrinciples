# Solución: Creación de Sesiones con SRP

## La Solución: Responsabilidades Separadas

La responsabilidad de "crear una sesión" se divide en clases enfocadas, cada una con **una única razón para cambiar**:

### 1️⃣ `Session.cs` - Entidad de Dominio

**Responsabilidad única**: Representar el estado y reglas de negocio de una sesión.

```csharp
// Solo cambia si cambian las reglas de negocio de Session
public sealed class Session : AuditableEntity
{
    public static Result<Session> Create(/* params */) { }
    public Result Approve() { }
    public Result Cancel() { }
}
```

**Razones para cambiar**:

- Nuevas reglas de validación de dominio
- Nuevos estados de sesión
- Nuevas propiedades de negocio

### 2️⃣ `CreateSessionCommand.cs` - Comando

**Responsabilidad única**: Representar la intención de crear una sesión (DTO).

```csharp
// Solo cambia si cambia el contrato de entrada
public sealed record CreateSessionCommand(
    string Title,
    string Speaker,
    DateTime Date,
    Guid CommunityId);
```

### 3️⃣ `CreateSessionCommandHandler.cs` - Orquestador

**Responsabilidad única**: Coordinar el flujo del caso de uso.

```csharp
// Solo cambia si cambia el flujo del caso de uso
public sealed class CreateSessionCommandHandler
{
    public async Task<Result<Session>> Handle(CreateSessionCommand command)
    {
        // 1. Crear entidad de dominio
        var session = Session.Create(...);

        // 2. Persistir
        await _repository.AddAsync(session);

        // 3. Notificar (responsabilidad delegada)
        await _notificationService.NotifySessionCreated(session);

        return session;
    }
}
```

**Razones para cambiar**:

- Cambios en los pasos del caso de uso
- Nuevas validaciones de negocio

### 4️⃣ Interfaces - Contratos de Infraestructura

**Responsabilidad única**: Definir contratos sin conocer implementación.

```csharp
// ISessionRepository: Solo cambia si cambia el contrato de persistencia
public interface ISessionRepository
{
    Task AddAsync(Session session);
    Task<Session?> GetByIdAsync(Guid id);
}

// INotificationService: Solo cambia si cambia el contrato de notificación
public interface INotificationService
{
    Task NotifySessionCreated(Session session);
}

// IUnitOfWork: Solo cambia si cambia el contrato de transacciones
public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
```

## 🎯 Beneficios Alcanzados

### Testabilidad

```csharp
[Fact]
public async Task Should_Create_Valid_Session()
{
    // Mock solo lo que necesitas - sin DB, sin SMTP, sin archivos
    var repository = Substitute.For<ISessionRepository>();
    var notifications = Substitute.For<INotificationService>();
    var unitOfWork = Substitute.For<IUnitOfWork>();

    var handler = new CreateSessionCommandHandler(repository, notifications, unitOfWork);

    var result = await handler.Handle(command);

    result.IsSuccess.Should().BeTrue();
}
```

### Flexibilidad

Puedes cambiar la implementación sin tocar el handler:

```csharp
// Cambiar SQL → Cosmos DB
services.AddScoped<ISessionRepository, CosmosSessionRepository>();

// Cambiar Email → Slack
services.AddScoped<INotificationService, SlackNotificationService>();

// Agregar Application Insights sin tocar código existente
services.Decorate<INotificationService, TelemetryNotificationDecorator>();
```

### Mantenibilidad

- Cambios en validación → Solo tocas `Session.cs`
- Cambios en persistencia → Solo tocas la implementación del repositorio
- Cambios en correo → Solo tocas la implementación de notificación
- Cambios en el flujo → Solo tocas el handler

## 🔗 Patrones Aplicados

- **Command Pattern**: `CreateSessionCommand` + `CreateSessionCommandHandler`
- **Repository Pattern**: `ISessionRepository`
- **Dependency Inversion**: Handler depende de abstracciones
- **Factory Method**: `Session.Create()`
- **Domain Events**: `SessionCreatedDomainEvent`
