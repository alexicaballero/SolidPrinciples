# Solución: Sesiones con Diseño Basado en Políticas

## La Solución: Strategy Pattern para Reglas Variables

En lugar de jerarquía con precondiciones fortalecidas, usamos **estrategias inyectadas** para políticas de registro y cancelación.

### ANTES: Subtipos que Rompen Contratos

```csharp
Session (abstract)
  ├─ PrivateSession (fortalece precondiciones)
  └─ PublicSession (debilita postcondiciones)
```

### DESPUÉS: Políticas como Estrategias

```csharp
// Una clase con políticas inyectadas
public sealed class Session
{
    private readonly IRegistrationPolicy _registrationPolicy;
    private readonly ICancellationPolicy _cancellationPolicy;

    public Session(
        IRegistrationPolicy registrationPolicy,
        ICancellationPolicy cancellationPolicy)
    {
        _registrationPolicy = registrationPolicy;
        _cancellationPolicy = cancellationPolicy;
    }

    public Result Register(Member member)
    {
        // Delega validación a la política
        return _registrationPolicy.CanRegister(this, member)
            ? RegisterMember(member)
            : Result.Failure("Registro no permitido por la política");
    }

    public Result Cancel()
    {
        // Delega validación a la política
        return _cancellationPolicy.CanCancel(this)
            ? CancelSession()
            : Result.Failure("Cancelación no permitida por la política");
    }
}
```

### Estrategias de Registro

```csharp
public interface IRegistrationPolicy
{
    bool CanRegister(Session session, Member member);
}

// Política abierta: Cualquiera puede registrarse
public sealed class OpenRegistrationPolicy : IRegistrationPolicy
{
    public bool CanRegister(Session session, Member member) => true;
}

// Política restringida: Solo miembros verificados
public sealed class VerifiedOnlyRegistrationPolicy : IRegistrationPolicy
{
    public bool CanRegister(Session session, Member member) =>
        member.IsVerified;
}

// Política por invitación
public sealed class InvitationOnlyRegistrationPolicy : IRegistrationPolicy
{
    public bool CanRegister(Session session, Member member) =>
        session.InvitedMembers.Contains(member.Id);
}
```

### Estrategias de Cancelación

```csharp
public interface ICancellationPolicy
{
    bool CanCancel(Session session);
}

// Política flexible: Siempre se puede cancelar
public sealed class AlwaysCancellablePolicy : ICancellationPolicy
{
    public bool CanCancel(Session session) => true;
}

// Política estricta: No se puede cancelar con asistentes
public sealed class NoAttendeeCancellationPolicy : ICancellationPolicy
{
    public bool CanCancel(Session session) => !session.Attendees.Any();
}

// Política por tiempo: Solo hasta 24h antes
public sealed class TimeBasedCancellationPolicy : ICancellationPolicy
{
    public bool CanCancel(Session session) =>
        session.ScheduledAt > DateTime.UtcNow.AddHours(24);
}
```

## 🎯 Beneficios Alcanzados

### Todas las Sesiones Son Substituibles

```csharp
// Crea sesiones con diferentes políticas
Session publicSession = new Session(
    new OpenRegistrationPolicy(),
    new NoAttendeeCancellationPolicy());

Session privateSession = new Session(
    new VerifiedOnlyRegistrationPolicy(),
    new AlwaysCancellablePolicy());

Session inviteOnly = new Session(
    new InvitationOnlyRegistrationPolicy(),
    new TimeBasedCancellationPolicy());

// TODAS son genuinamente substituibles
List<Session> sessions = new() { publicSession, privateSession, inviteOnly };

foreach (var session in sessions)
{
    // Funciona para TODAS las sesiones sin verificar tipos
    var result = session.Register(member);

    if (result.IsFailure)
        Console.WriteLine(result.Error); // Manejo explícito
}
```

### Composición Flexible de Políticas

```csharp
// Combina políticas como necesites
services.AddScoped<Session>(sp => new Session(
    new VerifiedOnlyRegistrationPolicy(),
    new TimeBasedCancellationPolicy()));

// Cambia políticas sin modificar Session
services.AddScoped<Session>(sp => new Session(
    new OpenRegistrationPolicy(),
    new AlwaysCancellablePolicy()));
```

### Testabilidad Total

```csharp
[Fact]
public void Should_Reject_Registration_When_Policy_Denies()
{
    // Inyecta política mock
    var policy = Substitute.For<IRegistrationPolicy>();
    policy.CanRegister(Arg.Any<Session>(), Arg.Any<Member>())
          .Returns(false);

    var session = new Session(policy, new AlwaysCancellablePolicy());

    var result = session.Register(member);

    result.IsFailure.Should().BeTrue();
}
```

### Agregar Nuevas Políticas Sin Modificar Session

```csharp
// Nueva política: Solo primeros 50 registros
public sealed class CapacityLimitedRegistrationPolicy : IRegistrationPolicy
{
    private readonly int _maxCapacity;

    public CapacityLimitedRegistrationPolicy(int maxCapacity = 50)
    {
        _maxCapacity = maxCapacity;
    }

    public bool CanRegister(Session session, Member member) =>
        session.Attendees.Count < _maxCapacity;
}

// CERO cambios a Session.cs
```

## 🔗 Patrones Aplicados

- **Strategy Pattern**: Políticas de registro y cancelación como objetos
- **Dependency Injection**: Políticas inyectadas vía constructor
- **Result Pattern**: Errores explícitos en lugar de excepciones
- **Open/Closed Principle**: Nuevas políticas sin modificar Session
- **Liskov Substitution**: Todas las sesiones son intercambiables
