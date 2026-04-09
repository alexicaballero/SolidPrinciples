# Solución: Miembros con Diseño Basado en Capacidades

## La Solución: Composición sobre Herencia

En lugar de una jerarquía frágil, usamos **composición** para representar capacidades opcionales que los miembros pueden tener.

### ANTES: Jerarquía que Rompe LSP

```csharp
Member (abstract)
  └─ GuestMember (throws NotImplementedException)
      └─ ActiveMember (funciona) ✓
```

### DESPUÉS: Composición de Capacidades

```csharp
// Una sola clase con capacidades opcionales
public sealed class Member
{
    public MemberRole Role { get; private set; }
    public bool CanCreateSessions => Role == MemberRole.Active;
    public bool CanVote => Role == MemberRole.Active;

    // Métodos que VERIFICAN capacidades en lugar de lanzar excepciones
    public Result CreateSession(string title)
    {
        if (!CanCreateSessions)
            return Result.Failure("Los miembros invitados no pueden crear sesiones");

        // Lógica de creación...
        return Result.Success();
    }

    public Result VoteOnProposal(Guid proposalId)
    {
        if (!CanVote)
            return Result.Failure("Los miembros invitados no pueden votar");

        // Lógica de votación...
        return Result.Success();
    }
}
```

## 🎯 Beneficios Alcanzados

### Sin Herencia Frágil

```csharp
// No hay subtipos que lancen NotImplementedException
// No hay verificaciones "if (member is ActiveMember)"
// No hay polimorfismo roto

Member guest = Member.CreateGuest(...);
Member active = Member.CreateActive(...);

// Ambos son REALMENTE substituibles
var members = new List<Member> { guest, active };

foreach (var member in members)
{
    // Funciona para TODOS los miembros
    var result = member.CreateSession("SOLID Principles");

    if (result.IsFailure)
        Console.WriteLine(result.Error); // Manejo explícito de error
}
```

### Verificación de Capacidades Explícita

```csharp
// Pregunta ANTES de intentar
if (member.CanCreateSessions)
{
    member.CreateSession("Clean Architecture");
}
else
{
    Console.WriteLine("Este miembro no puede crear sesiones");
}

// O usa el patrón Result
var result = member.CreateSession( "Domain-Driven Design");
if (result.IsSuccess)
{
    // Sesión creada
}
else
{
    // Manejo de error: result.Error
}
```

### Strategy Pattern para Comportamiento Variable

```csharp
// Si el comportamiento varía MUCHO entre tipo de miembros,
// usa inyección de estrategias en lugar de herencia

public interface IMemberPermissionPolicy
{
    bool CanCreateSession(Member member);
    bool CanVote(Member member);
}

public sealed class Member
{
    private readonly IMemberPermissionPolicy _policy;

    public Member(IMemberPermissionPolicy policy)
    {
        _policy = policy;
    }

    public Result CreateSession(string title)
    {
        if (!_policy.CanCreateSession(this))
            return Result.Failure("No tienes permisos");

        // ...
    }
}

// Diferentes políticas sin herencia
public sealed class GuestPermissionPolicy : IMemberPermissionPolicy { }
public sealed class ActivePermissionPolicy : IMemberPermissionPolicy { }
public sealed class AdminPermissionPolicy : IMemberPermissionPolicy { }
```

## 🔗 Patrones Aplicados

- **Composition over Inheritance**: Capacidades como propiedades/estrategias
- **Result Pattern**: Errores explícitos en lugar de excepciones
- **Strategy Pattern**: Políticas de permisos inyectadas
- **Guard Clauses**: Validación temprana de capacidades
