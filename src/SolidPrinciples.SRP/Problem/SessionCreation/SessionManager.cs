using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace SolidPrinciples.SRP.Problem;

/// <summary>
/// PROBLEMA: Esta clase Dios maneja sesiones pero se encarga de CINCO responsabilidades diferentes:
/// 1. Validación de entrada
/// 2. Persistencia de base de datos (SQL directo)
/// 3. Notificación por correo electrónico
/// 4. Registro en archivo
/// 5. Orquestación comercial
///
/// Tiene CINCO razones para cambiar — cualquier modificación en las reglas de validación, esquema de base de datos,
/// plantillas de correo electrónico, formato de registro o flujo comercial requiere tocar esta clase única.
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - El nombre de la clase "SessionManager" es vago — ¿qué significa "manage"?
/// - Importa espacios de nombres de acceso a datos, correo electrónico y E/S de archivos
/// - El constructor necesita una cadena de conexión, host SMTP Y ruta de archivo de registro
/// - Las pruebas requieren una base de datos real, servidor SMTP y sistema de archivos escribible
/// - Agregar un nuevo canal de notificación (p. ej., Slack) significa modificar esta clase
/// </remarks>
public sealed class SessionManager
{
  private readonly string _connectionString;
  private readonly string _smtpHost;
  private readonly int _smtpPort;
  private readonly string _logFilePath;

  public SessionManager(
      string connectionString,
      string smtpHost,
      int smtpPort,
      string logFilePath)
  {
    _connectionString = connectionString;
    _smtpHost = smtpHost;
    _smtpPort = smtpPort;
    _logFilePath = logFilePath;
  }

  /// <summary>
  /// Crea una sesión — pero valida, persiste, notifica y registra en un solo método.
  /// </summary>
  public void CreateSession(string title, string speaker, DateTime date, Guid communityId)
  {
    // VIOLACIÓN: Responsabilidad 1 — Validación de entrada mezclada con orquestación
    if (string.IsNullOrWhiteSpace(title))
      throw new ArgumentException("Se requiere un título.", nameof(title));

    if (string.IsNullOrWhiteSpace(speaker))
      throw new ArgumentException("Se requiere un orador.", nameof(speaker));

    if (date <= DateTime.Now)
      throw new ArgumentException("La fecha de la sesión debe estar en el futuro.", nameof(date));

    // VIOLACIÓN: Responsabilidad 2 — Acceso directo a la base de datos con SQL crudo
    var sessionId = Guid.NewGuid();
    var sql = $"""
            INSERT INTO Sessions (Id, Title, Speaker, ScheduledAt, CommunityId, Status, CreatedAt)
            VALUES ('{sessionId}', '{title}', '{speaker}', '{date:yyyy-MM-dd HH:mm}', '{communityId}', 'Programada', GETUTCDATE())
            """;
    ExecuteSql(sql);

    // VIOLACIÓN: Responsabilidad 3 — Obtención del nombre de la comunidad para notificación (más acceso a BD)
    var communitySql = $"SELECT Name FROM Communities WHERE Id = '{communityId}'";
    var communityName = ExecuteScalarSql(communitySql);

    // VIOLACIÓN: Responsabilidad 4 — Lógica de notificación por correo electrónico incrustada aquí
    SendEmailNotification(
        to: "admin@gathering.com",
        subject: $"Nueva sesión en {communityName}",
        body: $"Una nueva sesión '{title}' de {speaker} ha sido programada para {date:dd 'de' MMMM 'de' yyyy}.");

    // VIOLACIÓN: Responsabilidad 5 — Registro basado en archivos incrustado aquí
    var logEntry = $"[{DateTime.UtcNow:O}] Sesión creada: {sessionId} - {title} de {speaker}";
    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
  }

  /// <summary>
  /// Cancela una sesión — mezclando persistencia, notificación y registro nuevamente.
  /// </summary>
  public void CancelSession(Guid sessionId, string reason)
  {
    // VIOLACIÓN: Mismo patrón — SQL directo, correo electrónico y registro en un solo lugar
    var sql = $"UPDATE Sessions SET Status = 'Cancelada' WHERE Id = '{sessionId}'";
    ExecuteSql(sql);

    SendEmailNotification(
        to: "admin@gathering.com",
        subject: "Sesión cancelada",
        body: $"La sesión {sessionId} ha sido cancelada. Razón: {reason}");

    var logEntry = $"[{DateTime.UtcNow:O}] Sesión cancelada: {sessionId} - Razón: {reason}";
    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
  }

  // --- Métodos de infraestructura que NO deberían vivir en una clase "manager" ---

  private void ExecuteSql(string sql)
  {
    // Simulado — en realidad esto abriría una SqlConnection
    // Este acoplamiento estrecho hace que la clase sea inprobable sin una base de datos real
  }

  private string ExecuteScalarSql(string sql)
  {
    // Simulado — devuelve un valor ficticio
    return "Comunidad de Ejemplo";
  }

  private void SendEmailNotification(string to, string subject, string body)
  {
    // Simulado — en realidad esto usaría SmtpClient
    // Este acoplamiento estrecho hace que la clase sea inprobable sin un servidor SMTP
  }
}
