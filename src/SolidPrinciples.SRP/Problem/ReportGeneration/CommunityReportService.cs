using System.Text;

namespace SolidPrinciples.SRP.Problem;

/// <summary>
/// PROBLEMA: Esta clase maneja TRES responsabilidades diferentes para informes de comunidad:
/// 1. Recuperación de datos (consulta la fuente de datos)
/// 2. Formateo de informe (construye salida HTML/CSV)
/// 3. Entrega de informe (envía por correo electrónico)
///
/// Si la fuente de datos cambia (SQL → API), el formateo cambia (HTML → PDF),
/// o el método de entrega cambia (correo electrónico → Slack), esta única clase debe modificarse.
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - El nombre del método "GenerateAndSendReport" usa "and" — una pista de múltiples responsabilidades
/// - La clase sabe sobre acceso a datos, formateo HTML Y envío de correo electrónico
/// - No puede probar la lógica de formateo sin tratar tambien con el acceso a datos
/// - Agregar un nuevo formato de salida requiere modificar esta clase
/// </remarks>
public sealed class CommunityReportService
{
  private readonly string _connectionString;
  private readonly string _smtpHost;

  public CommunityReportService(string connectionString, string smtpHost)
  {
    _connectionString = connectionString;
    _smtpHost = smtpHost;
  }

  /// <summary>
  /// Obtiene datos, los formatea como HTML y los envía por correo electrónico — todo en un método.
  /// </summary>
  public void GenerateAndSendReport(Guid communityId, string recipientEmail)
  {
    // VIOLACIÓN: Responsabilidad 1 — Lógica de recuperación de datos
    var sessions = FetchSessionData(communityId);

    // VIOLACIÓN: Responsabilidad 2 — Formateo de informe (construcción HTML)
    var html = new StringBuilder();
    html.Append("<html><body>");
    html.Append($"<h1>Informe de Comunidad</h1>");
    html.Append("<table><tr><th>Título</th><th>Orador</th><th>Fecha</th><th>Estado</th></tr>");

    foreach (var session in sessions)
    {
      html.Append($"<tr><td>{session.Title}</td><td>{session.Speaker}</td>");
      html.Append($"<td>{session.Date:yyyy-MM-dd}</td><td>{session.Status}</td></tr>");
    }

    html.Append("</table></body></html>");

    // VIOLACIÓN: Responsabilidad 3 — Lógica de entrega de correo electrónico
    SendEmail(recipientEmail, "Informe de Sesiones de Comunidad", html.ToString());
  }

  /// <summary>
  /// ¿Quiere CSV en lugar de HTML? Debe modificar ESTA clase.
  /// ¿Quiere enviar a través de Slack? Debe modificar ESTA clase.
  /// ¿Quiere leer desde una API? Debe modificar ESTA clase.
  /// </summary>
  public void GenerateAndSendCsvReport(Guid communityId, string recipientEmail)
  {
    var sessions = FetchSessionData(communityId);

    // VIOLACIÓN: Recuperación de datos duplicada + formateo diferente + misma entrega
    var csv = new StringBuilder();
    csv.AppendLine("Título,Orador,Fecha,Estado");
    foreach (var session in sessions)
    {
      csv.AppendLine($"{session.Title},{session.Speaker},{session.Date:yyyy-MM-dd},{session.Status}");
    }

    SendEmail(recipientEmail, "Informe de Sesiones de Comunidad (CSV)", csv.ToString());
  }

  private List<SessionData> FetchSessionData(Guid communityId)
  {
    // Recuperación de datos simulada — en realidad, consulta SQL sin procesar u ORM
    return
    [
        new("Taller SOLID", "Jane Smith", DateTime.Today.AddDays(7), "Programado"),
        new("Profundización de DDD", "John Doe", DateTime.Today.AddDays(14), "Programado")
    ];
  }

  private void SendEmail(string to, string subject, string body)
  {
    // Simulado — usaría SmtpClient en producción
  }

  /// <summary>
  /// Registro de datos de sesión.
  /// </summary>
  private sealed record SessionData(string Title, string Speaker, DateTime Date, string Status);
}
