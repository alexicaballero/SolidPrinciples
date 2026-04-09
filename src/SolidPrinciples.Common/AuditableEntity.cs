namespace SolidPrinciples.Common;

/// <summary>
/// Clase base para entidades que rastrean creación, modificación y eventos de dominio.
/// </summary>
public abstract class AuditableEntity
{
  public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
  public DateTimeOffset? ModifiedAt { get; protected set; }

  private readonly List<IDomainEvent> _domainEvents = [];

  public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

  protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

  public void ClearDomainEvents() => _domainEvents.Clear();
}
