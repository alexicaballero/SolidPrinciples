using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Problem.EntityHierarchy;

/// <summary>
/// Evento de dominio generado cuando se crea una sesión.
/// </summary>
public sealed record SessionCreatedEvent(Guid SessionId) : DomainEventBase(Guid.NewGuid());

/// <summary>
/// Evento de auditoría especial requerido por RestrictedEntity.
/// </summary>
public sealed record AuditEvent(string Action, Guid EntityId) : DomainEventBase(Guid.NewGuid());
