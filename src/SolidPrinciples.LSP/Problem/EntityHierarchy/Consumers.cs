namespace SolidPrinciples.LSP.Problem.EntityHierarchy;

/// <summary>
/// PROBLEMA: Despachador de eventos genérico que espera que TODAS las entidades honren el contrato.
/// </summary>
public sealed class EventDispatcher
{
    /// <summary>
    /// Despacha eventos de cualquier entidad.
    /// FALLA con RestrictedEntity si se levantó el tipo de evento incorrecto.
    /// </summary>
    public void DispatchEvents(Entity entity)
    {
        // Este código confía en el contrato de Entity:
        // - Raise() acepta cualquier IDomainEvent
        // - DomainEvents contiene todos los eventos levantados

        foreach (var domainEvent in entity.DomainEvents)
        {
            Console.WriteLine($"Dispatching: {domainEvent.GetType().Name}");
            // Procesar evento...
        }

        entity.ClearDomainEvents();
    }
}

/// <summary>
/// PROBLEMA: Repositorio que asume que las entidades auditables siempre tienen timestamps.
/// </summary>
public sealed class AuditRepository
{
    /// <summary>
    /// Consulta entidades creadas después de una fecha determinada.
    /// FALLA con BrokenAuditableEntity porque CreatedAt puede ser nulo.
    /// </summary>
    public IEnumerable<BrokenAuditableEntity> GetRecentEntities(
        List<BrokenAuditableEntity> entities,
        DateTimeOffset since)
    {
        return entities
            .Where(e => e.CreatedAt.HasValue && e.CreatedAt.Value > since) // ¡Verificación defensiva de nulo necesaria!
            .ToList();
    }
}
