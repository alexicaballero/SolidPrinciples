# Caso de Uso 2: Jerarquía de Sesiones

## El Problema: Precondiciones Fortalecidas y Postcondiciones Debilitadas

La jerarquía `Session` → `PrivateSession` → `PublicSession` viola LSP porque los subtipos **cambian los contratos** del tipo base.

### 🚩 Violación: Precondiciones Fortalecidas

```csharp
public abstract class Session
{
    // Contrato base: Cualquier miembro puede registrarse
    public abstract void Register(Member member);
}

// PrivateSession requiere MÁS que la clase base
public class PrivateSession : Session
{
    public override void Register(Member member)
    {
        // FORTALECE precondición: Solo miembros verificados
        if (!member.IsVerified)
        {
            throw new InvalidOperationException(
                "Solo miembros verificados pueden registrarse en sesiones privadas");
        }
        // ...
    }
}
```

**Problema**: Código que funciona con `Session` falla con `PrivateSession`.

### 🚩 Violación: Postcondiciones Debilitadas

```csharp
public abstract class Session
{
    // Contrato base: Cancel siempre tiene éxito
    public abstract void Cancel();
}

// PublicSession NO garantiza lo que Session promete
public class PublicSession : Session
{
    public override void Cancel()
    {
        // DEBILITA postcondición: Cancel puede fallar
        if (Attendees.Any())
        {
            throw new InvalidOperationException(
                "No se puede cancelar sesión pública con asistentes");
        }
        Status = SessionStatus.Cancelled;
    }
}
```

### 💥 Por Qué Esto Viola LSP

**Precondición fortalecida**:

```csharp
Session session = GetSession(id); // Podría ser PrivateSession
Member member = new Member { IsVerified = false };

// Funciona con Session base
// Falla con PrivateSession (requiere IsVerified = true)
session.Register(member); // Boom! Si es PrivateSession
```

**Postcondición debilitada**:

```csharp
Session session = GetSession(id); // Podría ser PublicSession

// El contrato dice: "Cancel siempre tiene éxito"
// PublicSession lo rompe: Puede lanzar excepción
session.Cancel(); // Boom! Si es PublicSession con asistentes
```

### 🎯 Señales de Advertencia

- ✖️ Subtipos que agregan validaciones más estrictas
- ✖️ Subtipos que lanzan excepciones donde el base no lo hace
- ✖️ Documentación: "Este método falla en algunos subtipos"
- ✖️ Reglas del negocio diferentes por subtipo
- ✖️ Try-catch específicos por tipo: `catch when (x is PublicSession)`

### 📖 Ver la Solución

Revisa `../../Solution/SessionHierarchy/` para ver el **diseño basado en políticas**:

- `Session.cs` - Clase con estrategias de validación inyectadas
- Políticas de registro y cancelación como objetos independientes
- Sin herencia frágil — composición de comportamiento
- Todos los objetos son **genuinamente substituibles**
