using SolidPrinciples.Common;

namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// Evento de dominio generado cuando se crea una nueva sesión.
/// </summary>
public sealed record SessionCreatedDomainEvent(Guid SessionId) : DomainEventBase(Guid.NewGuid());
