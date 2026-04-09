using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Solution.EntityHierarchy;

/// <summary>
/// SOLUCIÓN: La clase base Entity establece un contrato para eventos de dominio.
/// </summary>
/// <remarks>
/// Entity define un contrato claro e inquebrantable:
/// - Cualquier entidad puede levantar cualquier IDomainEvent
/// - Los eventos son recopilados y pueden limpiarse
/// - Sin restricciones de subtipos
///
/// Este contrato es preservado por TODOS los subtipos, habilitando:
/// - Despachadores de eventos genéricos
/// - Publicación de eventos del repositorio
/// - Manejo polimórfico de eventos de dominio
///
/// Patrón: Método Plantilla (Raise/Clear) con acceso protegido
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

    /// <summary>
    /// CORRECTO: Método protegido disponible para todos los subtipos sin restricciones.
    /// </summary>
    protected void Raise(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
