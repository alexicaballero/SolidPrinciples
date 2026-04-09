using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Problem.EntityHierarchy;

/// <summary>
/// PROBLEMA: La clase base Entity establece un contrato para eventos de dominio.
/// </summary>
/// <remarks>
/// Entity define un contrato claro:
/// - Cualquier entidad puede levantar eventos de dominio
/// - Los eventos pueden ser recopilados y limpiados
/// - Sin restricciones sobre qué eventos se pueden levantar
///
/// Este contrato habilita infraestructura genérica como:
/// - Despachadores de eventos que procesan entity.DomainEvents
/// - Repositorios que publican eventos después de SaveChanges
/// - Pipelines mediadores que reaccionan a eventos de dominio
/// </remarks>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
