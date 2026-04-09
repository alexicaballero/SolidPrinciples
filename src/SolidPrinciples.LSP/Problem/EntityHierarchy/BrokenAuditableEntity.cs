using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Problem.EntityHierarchy;

/// <summary>
/// VIOLACIÓN: BrokenAuditableEntity debilita las postcondiciones al permitir timestamps nulos.
/// </summary>
/// <remarks>
/// Esto viola LSP porque:
/// 1. Los clientes esperan que las entidades auditables tengan timestamps válidos
/// 2. BrokenAuditableEntity puede tener CreatedAt nulo
/// 3. Las consultas de auditoría del repositorio fallarán
///
/// Violación de invariante: Una entidad auditable SIEMPRE debe tener CreatedAt establecido.
/// </remarks>
public abstract class BrokenAuditableEntity : Entity
{
    // VIOLACIÓN: Timestamp anulable rompe el contrato auditable
    // Los clientes esperan que CreatedAt siempre tenga un valor para el registro de auditoría
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}

/// <summary>
/// Implementación concreta usando BrokenAuditableEntity
/// </summary>
public sealed class BrokenSession : BrokenAuditableEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;

    public static BrokenSession Create(string title)
    {
        var session = new BrokenSession
        {
            Id = Guid.NewGuid(),
            Title = title
            // PROBLEMA: ¡CreatedAt no está establecido! Invariante violado
        };

        session.Raise(new SessionCreatedEvent(session.Id));
        return session;
    }
}
