# Caso de Uso 1: Jerarquía de Miembros

## El Problema: Subtipos que Rompen Contratos

La jerarquía `Member` → `GuestMember` → `ActiveMember` viola LSP porque los subtipos **no pueden reemplazar** al tipo base sin romper el comportamiento esperado.

### 🚩 Violación: NotImplementedException

```csharp
public abstract class Member
{
    public abstract void CreateSession(string title);
    public abstract void VoteOnProposal(Guid proposalId);
}

// GuestMember lanza excepciones = NO es substituible
public class GuestMember : Member
{
    // Hereda el método pero no puede cumplir el contrato
    public override void CreateSession(string title)
    {
        throw new NotImplementedException("Los invitados no pueden crear sesiones");
    }

    public override void VoteOnProposal(Guid proposalId)
    {
        throw new NotImplementedException("Los invitados no pueden votar");
    }
}
```

### 💥 Por Qué Esto Viola LSP

**NO es substituible**:

```csharp
Member member = GetMember(userId); // Podría ser GuestMember

// Este código puede lanzar excepciones en tiempo de ejecución
member.CreateSession("SOLID Principles"); // Boom! Si es GuestMember

// El código que funciona con Member NO funciona con GuestMember
```

**Requiere verificación de tipo**:

```csharp
// Code smell: Necesitas verificar el tipo antes de usar
if (member is not GuestMember)
{
    member.CreateSession("SOLID Principles");
}

// LSP roto: El polimorfismo no funciona
```

### 🎯 Señales de Advertencia

- ✖️ `NotImplementedException` en métodos heredados
- ✖️ Verificaciones `if (x is SubType)` antes de llamar métodos
- ✖️ Comentarios: "No usar este método en GuestMember"
- ✖️ Subtipos que no pueden cumplir el contrato del tipo base
- ✖️ Try-catch alrededor de llamadas porque "a veces falla"

### 📖 Ver la Solución

Revisa `../../Solution/MemberHierarchy/` para ver el **diseño basado en capacidades**:

- `Member.cs` - Composición sobre herencia
- Clases con capacidades opcionales en lugar de jerarquías frágiles
- Estrategias inyectadas para comportamiento variable
- Todos los miembros son **genuinamente substituibles**
