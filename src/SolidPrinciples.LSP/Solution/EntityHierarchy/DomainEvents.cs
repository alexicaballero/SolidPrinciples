using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Solution.EntityHierarchy;

/// <summary>
/// Evento de dominio generado cuando se crea una sesión.
/// </summary>
public sealed record SessionCreatedDomainEvent(Guid SessionId) : DomainEventBase(Guid.NewGuid());

/// <summary>
/// Evento de dominio generado cuando se actualiza una sesión.
/// </summary>
public sealed record SessionUpdatedDomainEvent(Guid SessionId) : DomainEventBase(Guid.NewGuid());

/// <summary>
/// Evento de dominio generado cuando se crea una comunidad.
/// </summary>
public sealed record CommunityCreatedDomainEvent(Guid CommunityId) : DomainEventBase(Guid.NewGuid());
