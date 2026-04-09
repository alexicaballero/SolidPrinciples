using SolidPrinciples.Common;

namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: La entidad de sesión tiene UNA responsabilidad — representar y proteger
/// el estado del dominio. Valida sus propios invariantes a través de un método de fábrica y genera eventos de dominio.
///
/// NO hace:
/// - Saber cómo se persiste (sin SQL, sin cadenas de conexión)
/// - Enviar correos electrónicos o notificaciones
/// - Registrar nada
/// - Conocer sobre infraestructura
/// </summary>
/// <remarks>
/// Beneficios:
/// - Testeable con cero infraestructura — solo llama a Create() y aserta
/// - Los cambios en persistencia, notificación o registro nunca tocan esta clase
/// - El método de fábrica garantiza que una sesión siempre esté en un estado válido
/// </remarks>
public sealed class Session : AuditableEntity
{
  public Guid Id { get; private set; }
  public Guid CommunityId { get; private set; }
  public string Title { get; private set; } = string.Empty;
  public string Speaker { get; private set; } = string.Empty;
  public DateTimeOffset ScheduledAt { get; private set; }
  public SessionStatus Status { get; private set; }

  private Session() { }

  /// <summary>
  /// Método de fábrica — valida invariantes y devuelve un Result en lugar de lanzar excepciones.
  /// </summary>
  public static Result<Session> Create(
      Guid communityId,
      string title,
      string speaker,
      DateTimeOffset scheduledAt)
  {
    // CORRECTO: La validación es responsabilidad de la entidad — protegiendo sus propios invariantes
    if (string.IsNullOrWhiteSpace(title))
      return Result.Failure<Session>(SessionErrors.TitleRequired);

    if (string.IsNullOrWhiteSpace(speaker))
      return Result.Failure<Session>(SessionErrors.SpeakerRequired);

    if (scheduledAt <= DateTimeOffset.UtcNow)
      return Result.Failure<Session>(SessionErrors.DateMustBeFuture);

    var session = new Session
    {
      Id = Guid.NewGuid(),
      CommunityId = communityId,
      Title = title,
      Speaker = speaker,
      ScheduledAt = scheduledAt,
      Status = SessionStatus.Scheduled
    };

    // CORRECTO: Evento de dominio para efectos secundarios — la entidad no envía correos electrónicos por sí misma
    session.Raise(new SessionCreatedDomainEvent(session.Id));

    return Result.Success(session);
  }
}

/// <summary>
/// Enumeración que representa el estado del ciclo de vida de una sesión.
/// </summary>
public enum SessionStatus
{
  Scheduled,
  Confirmed,
  Canceled,
  Completed
}
