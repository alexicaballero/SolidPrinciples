# Caso de Uso 1: Command Handler con Dependencias Directas

## El Problema: `new` en Lógica de Aplicación

La clase `CreateSessionHandler` viola DIP porque **instancia directamente** sus dependencias de infraestructura con `new`.

### 🚩 Dependencias Hardcodeadas

```csharp
// Handler de alto nivel depende de clases concretas de bajo nivel
public sealed class CreateSessionHandler
{
    public void Handle(CreateSessionCommand command)
    {
        // VIOLA DIP: Instancia SqlSessionRepository con new
        var repository = new SqlSessionRepository(
            "Server=localhost;Database=Gathering;...");

        // VIOLA DIP: Instancia EmailNotificationSender con new
        var emailSender = new EmailNotificationSender(
            smtpHost: "smtp.gmail.com",
            port: 587);

        // Lógica de negocio...
        var session = new Session(command.Title, command.Speaker, ...);
        repository.Add(session);
        emailSender.SendEmail(command.AdminEmail, "Sesión creada");
    }
}
```

### 💥 Problemas de este Diseño

**Acoplamiento estrecho**:

- Handler conoce `SqlSessionRepository` concreta
- Handler conoce `EmailNotificationSender` concreta
- Imposible cambiar a MongoDB, CosmosDB, Slack sin modificar Handler

**NO testeable en unidad**:

```csharp
[Fact]
public void Should_Create_Session()
{
    var handler = new CreateSessionHandler();

    // NO puedes ejecutar esto sin:
    // - SQL Server corriendo
    // - Servidor SMTP configurado
    // - Credenciales válidas

    handler.Handle(command); // Requiere infraestructura real
}
```

**Violaciones de arquitectura en capas**:

```csharp
// Capa Application importa capa Infrastructure
using Gathering.Infrastructure.Persistence;  //
using Gathering.Infrastructure.Messaging;    //

// La flecha de dependencia apunta HACIA ABAJO (mal)
Application → Infrastructure
```

### 🎯 Señales de Advertencia

- ✖️ Palabra clave `new` en lógica de negocio
- ✖️ Imports de `System.Data.SqlClient`, `System.Net.Mail`
- ✖️ Connection strings en código de aplicación
- ✖️ Pruebas unitarias que requieren DB/SMTP
- ✖️ Imposible mockear dependencias

### 📖 Ver la Solución

Revisa `../../Solution/CommandHandler/` para ver **inversión de dependencias**:

- `CreateSessionHandler` depende de abstracciones (`ISessionRepository`, `INotificationSender`)
- Dependencias inyectadas vía constructor
- Testeable con mocks/in-memory
- Application NO conoce Infrastructure
