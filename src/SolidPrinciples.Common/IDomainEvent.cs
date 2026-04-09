namespace SolidPrinciples.Common;

/// <summary>
/// Interfaz marcadora para eventos de dominio.
/// </summary>
public interface IDomainEvent
{
  Guid Id { get; }
  DateTimeOffset OccurredAt { get; }
}

/// <summary>
/// Registro base para eventos de dominio con marca de tiempo automática.
/// </summary>
public abstract record DomainEventBase(Guid Id) : IDomainEvent
{
  public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}
