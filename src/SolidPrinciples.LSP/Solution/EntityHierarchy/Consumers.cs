namespace SolidPrinciples.LSP.Solution.EntityHierarchy;

/// <summary>
/// SOLUCIÓN: Despachador de eventos genérico que funciona con CUALQUIER subtipo de Entity.
/// </summary>
/// <remarks>
/// Este despachador demuestra LSP en acción:
/// - Acepta cualquier Entity (tipo base)
/// - Funciona correctamente con Session, Community o cualquier entidad futura
/// - No se necesita verificación de tipos
/// - Sin casos especiales
///
/// POR QUÉ ESTO FUNCIONA: Cada subtipo honra el contrato de Entity.
/// </remarks>
public sealed class EventDispatcher
{
    /// <summary>
    /// CORRECTO: Despacha eventos de forma segura desde cualquier subtipo de entidad.
    /// </summary>
    public void DispatchEvents(Entity entity)
    {
        // Este código funciona con Entity, AuditableEntity, Session, Community
        // o CUALQUIER subtipo futuro porque todos honran el contrato

        foreach (var domainEvent in entity.DomainEvents)
        {
            Console.WriteLine($"Dispatching: {domainEvent.GetType().Name}");
            // Publicar al bus de mensajes, activar manejadores, etc.
        }

        entity.ClearDomainEvents();
    }
}

/// <summary>
/// SOLUCIÓN: Repositorio genérico que funciona con cualquier AuditableEntity.
/// </summary>
/// <remarks>
/// Este repositorio demuestra los beneficios de LSP:
/// - Restricción genérica: where T : AuditableEntity
/// - Confía en que CreatedAt es SIEMPRE válido (no anulable)
/// - Funciona con Session, Community o cualquier entidad auditable futura
/// - No se necesitan verificaciones defensivas de nulos
///
/// Patrón: Repositorio con restricción genérica aprovechando jerarquía compatible con LSP
/// </remarks>
public sealed class AuditableRepository<T> where T : AuditableEntity
{
    private readonly List<T> _entities = [];

    public void Add(T entity)
    {
        _entities.Add(entity);
    }

    /// <summary>
    /// CORRECTO: No se necesitan verificaciones de nulos - CreatedAt está garantizado que tiene un valor.
    /// </summary>
    public IReadOnlyList<T> GetRecentEntities(DateTimeOffset since)
    {
        return _entities
            .Where(e => e.CreatedAt > since) // ¡Seguro! No se necesita verificación de nulos
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Obtiene entidades modificadas después de una fecha determinada.
    /// </summary>
    public IReadOnlyList<T> GetUpdatedEntities(DateTimeOffset since)
    {
        return _entities
            .Where(e => e.UpdatedAt.HasValue && e.UpdatedAt.Value > since)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// CORRECTO: El despacho de eventos funciona para CUALQUIER tipo de entidad.
    /// </summary>
    public void PublishEvents(EventDispatcher dispatcher)
    {
        foreach (var entity in _entities)
        {
            // Llamada polimórfica segura - El contrato de Entity es honrado
            dispatcher.DispatchEvents(entity);
        }
    }
}
