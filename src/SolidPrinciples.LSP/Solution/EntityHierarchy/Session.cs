using SolidPrinciples.Common;

namespace SolidPrinciples.LSP.Solution.EntityHierarchy;

/// <summary>
/// SOLUCIÓN: Session es una entidad concreta sellada que usa la jerarquía correctamente.
/// </summary>
/// <remarks>
/// Session respeta LSP porque:
/// 1. Usa Raise() exactamente como Entity lo define
/// 2. Hereda el comportamiento de auditoría de AuditableEntity sin modificación
/// 3. Está sellada para prevenir más violaciones en la jerarquía
/// 4. Puede usarse donde Entity o AuditableEntity se espera
///
/// Patrón: Implementación concreta final (sealed class)
/// </remarks>
public sealed class Session : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Speaker { get; private set; } = string.Empty;
    public DateTimeOffset ScheduledAt { get; private set; }

    private Session() { }

    /// <summary>
    /// CORRECTO: Método factory que crea una Session válida y levanta evento de dominio.
    /// </summary>
    public static Result<Session> Create(
        string title,
        string speaker,
        DateTimeOffset scheduledAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Session>(new Error(
                "Session.InvalidTitle",
                "Session title cannot be empty"));

        if (string.IsNullOrWhiteSpace(speaker))
            return Result.Failure<Session>(new Error(
                "Session.InvalidSpeaker",
                "Speaker name cannot be empty"));

        if (scheduledAt <= DateTimeOffset.UtcNow)
            return Result.Failure<Session>(new Error(
                "Session.InvalidSchedule",
                "Session must be scheduled in the future"));

        var session = new Session
        {
            Id = Guid.NewGuid(),
            Title = title,
            Speaker = speaker,
            ScheduledAt = scheduledAt
        };

        // CORRECTO: Usa Raise() heredado sin ninguna restricción
        session.Raise(new SessionCreatedDomainEvent(session.Id));

        return Result.Success(session);
    }

    /// <summary>
    /// Actualiza los detalles de la sesión y levanta el evento apropiado.
    /// </summary>
    public Result Update(string title, string speaker)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(new Error(
                "Session.InvalidTitle",
                "Session title cannot be empty"));

        if (string.IsNullOrWhiteSpace(speaker))
            return Result.Failure(new Error(
                "Session.InvalidSpeaker",
                "Speaker name cannot be empty"));

        Title = title;
        Speaker = speaker;

        // CORRECTO: Usa métodos heredados de Entity y AuditableEntity
        MarkAsUpdated();
        Raise(new SessionUpdatedDomainEvent(Id));

        return Result.Success();
    }
}
