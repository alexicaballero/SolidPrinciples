namespace SolidPrinciples.Common;

/// <summary>
/// Entidad de dominio que representa una sesión (charla) dentro de una comunidad de práctica.
/// </summary>
/// <remarks>
/// Una sesión es un evento donde un miembro de la comunidad comparte conocimiento.
/// Incluye título, descripción, orador, fecha/hora y estado.
/// </remarks>
public sealed class Session : AuditableEntity
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid CommunityId { get; private set; }
    public Guid? SpeakerId { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public SessionStatus Status { get; private set; }

    // Constructor privado - usar factory method
    private Session() { }

    /// <summary>
    /// Crea una nueva sesión propuesta.
    /// </summary>
    public static Result<Session> Create(string title, string description, Guid communityId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Session>(new Error("Session.Title", "El título es requerido"));

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Session>(new Error("Session.Description", "La descripción es requerida"));

        if (communityId == Guid.Empty)
            return Result.Failure<Session>(new Error("Session.CommunityId", "El ID de comunidad es inválido"));

        var session = new Session
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            CommunityId = communityId,
            Status = SessionStatus.Proposed
        };

        return Result.Success(session);
    }

    /// <summary>
    /// Programa la sesión con un orador y fecha específica.
    /// </summary>
    public Result Schedule(Guid speakerId, DateTime scheduledAt)
    {
        if (Status != SessionStatus.Proposed)
            return Result.Failure(new Error("Session.Schedule", "Solo se pueden programar sesiones propuestas"));

        if (speakerId == Guid.Empty)
            return Result.Failure(new Error("Session.SpeakerId", "El ID del orador es inválido"));

        if (scheduledAt <= DateTime.UtcNow)
            return Result.Failure(new Error("Session.ScheduledAt", "La fecha debe ser futura"));

        SpeakerId = speakerId;
        ScheduledAt = scheduledAt;
        Status = SessionStatus.Scheduled;
        ModifiedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}

/// <summary>
/// Estados posibles de una sesión.
/// </summary>
public enum SessionStatus
{
    Proposed,
    Scheduled,
    Completed,
    Cancelled
}
