using Microsoft.Extensions.Logging;
using SolidPrinciples.Common;

namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Este controlador tiene UNA responsabilidad — orquestar el caso de uso "crear sesión".
///
/// Compáralo con la clase Dios Problem/SessionManager.cs:
/// - SessionManager hizo validación, SQL, correo electrónico, registro y orquestación
/// - Este controlador SÓLO coordina: delega cada preocupación en un colaborador enfocado
///
/// Cada dependencia tiene su propia responsabilidad única:
/// - Entidad de sesión → valida reglas comerciales
/// - ISessionRepository → persiste la sesión
/// - INotificationService → envía notificaciones
/// - ILogger → maneja el registro
/// - IUnitOfWork → gestiona transacciones
/// </summary>
/// <remarks>
/// Beneficios:
/// - Testeable: inyecta mocks para cada dependencia
/// - Extensible: cambia el canal de notificación sin tocar esta clase
/// - Enfocado: esta clase solo cambia si el flujo de orquestación cambia
/// </remarks>
public sealed class CreateSessionCommandHandler(
    ISessionRepository sessionRepository,
    INotificationService notificationService,
    IUnitOfWork unitOfWork,
    ILogger<CreateSessionCommandHandler> logger)
{
  /// <summary>
  /// Maneja la creación de una nueva sesión.
  /// </summary>
  public async Task<Result<Guid>> HandleAsync(CreateSessionCommand command, CancellationToken ct = default)
  {
    // CORRECTO: Delegación a la entidad para validación (responsabilidad de la entidad)
    var result = Session.Create(
        command.CommunityId,
        command.Title,
        command.Speaker,
        command.ScheduledAt);

    if (result.IsFailure)
      return Result.Failure<Guid>(result.Error);

    var session = result.Value;

    // CORRECTO: Delegación al repositorio para persistencia (responsabilidad del repositorio)
    sessionRepository.Add(session);
    await unitOfWork.SaveChangesAsync(ct);

    // CORRECTO: Delegación al servicio de notificación (responsabilidad de notificación)
    await notificationService.SendSessionCreatedAsync(
        session.Id, session.Title, session.Speaker, ct);

    // CORRECTO: Delegación al logger (responsabilidad del registro)
    logger.LogInformation("Sesión {SessionId} creada: {Title} de {Speaker}",
        session.Id, session.Title, session.Speaker);

    return Result.Success(session.Id);
  }
}
