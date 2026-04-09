using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Problem.EntityHierarchy;

/// <summary>
/// VIOLACIÓN: RestrictedEntity fortalece las precondiciones en Raise().
/// </summary>
/// <remarks>
/// Esto viola LSP porque:
/// 1. La Entity base acepta CUALQUIER IDomainEvent
/// 2. RestrictedEntity requiere SOLO tipos AuditEvent
/// 3. El código que funciona con Entity fallará con RestrictedEntity
///
/// Fortalecimiento de precondición: La base requiere menos (cualquier evento),
/// pero el subtipo requiere más (solo eventos de auditoría).
///
/// Impacto:
/// - Los despachadores de eventos genéricos se rompen
/// - La publicación de eventos del repositorio falla con excepciones en tiempo de ejecución
/// - El código polimórfico no puede sustituir de forma segura RestrictedEntity por Entity
/// </remarks>
public abstract class RestrictedEntity : Entity
{
    protected new void Raise(IDomainEvent domainEvent)
    {
        // VIOLACIÓN: Fortaleciendo precondición
        // La base acepta cualquier IDomainEvent, esto requiere AuditEvent
        if (domainEvent is not AuditEvent)
            throw new ArgumentException(
                "RestrictedEntity only accepts AuditEvent instances",
                nameof(domainEvent));

        base.Raise(domainEvent);
    }
}

/// <summary>
/// Implementación concreta usando RestrictedEntity
/// </summary>
public sealed class RestrictedSession : RestrictedEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;

    public static RestrictedSession Create(string title)
    {
        var session = new RestrictedSession
        {
            Id = Guid.NewGuid(),
            Title = title
        };

        return session;
    }

    /// <summary>
    /// Método auxiliar público para exponer la violación de RestrictedEntity.Raise para pruebas.
    /// </summary>
    public void RaiseEvent(IDomainEvent domainEvent)
    {
        // Llama al método Raise restringido de RestrictedEntity
        var method = typeof(RestrictedEntity).GetMethod(
            "Raise",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(this, [domainEvent]);
    }
}
