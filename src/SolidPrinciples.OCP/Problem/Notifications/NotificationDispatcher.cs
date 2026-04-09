namespace SolidPrinciples.OCP.Problem;

/// <summary>
/// PROBLEMA: Este despachador de notificaciones utiliza una cadena if-else para decidir
/// qué canal de notificación usar. Agregar un nuevo canal (push, Teams, webhook)
/// requiere modificar esta clase.
///
/// La clase NO está cerrada para modificación — cada nuevo tipo de notificación
/// significa editar este código existente y funcional.
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - Cadena if-else creciente que verifica una cadena de "tipo"
/// - El método conoce CADA implementación de notificación (SMTP, SMS, Slack)
/// - Todos los canales están estrechamente acoplados en un lugar
/// - Agregar notificaciones por webhook requiere tocar el código de correo electrónico y SMS
/// </remarks>
public sealed class NotificationDispatcher
{
  /// <summary>
  /// Envía una notificación a través del canal especificado.
  /// Cada nuevo canal requiere modificar este método.
  /// </summary>
  public string Send(string channel, string recipient, string message)
  {
    // VIOLACIÓN: Cadena if-else que crece con cada nuevo canal de notificación
    if (channel.Equals("email", StringComparison.OrdinalIgnoreCase))
    {
      // Envío de correo electrónico simulado
      return $"CORREO ELECTRÓNICO a {recipient}: {message}";
    }
    else if (channel.Equals("sms", StringComparison.OrdinalIgnoreCase))
    {
      // Envío de SMS simulado
      var truncated = message.Length > 160 ? message[..160] : message;
      return $"SMS a {recipient}: {truncated}";
    }
    else if (channel.Equals("slack", StringComparison.OrdinalIgnoreCase))
    {
      // Mensaje de Slack simulado
      return $"SLACK a #{recipient}: *{message}*";
    }
    // VIOLACIÓN: ¿Necesitas notificaciones push? ¿Microsoft Teams? ¿Webhooks?
    // Debes modificar ESTA clase, arriesgando roturas en correo electrónico, SMS y Slack.
    else
    {
      throw new NotSupportedException($"El canal de notificación '{channel}' no es compatible.");
    }
  }

  /// <summary>
  /// Emite a todos los canales — las cadenas if-else se multiplican.
  /// </summary>
  public List<string> Broadcast(string message, IReadOnlyList<(string Channel, string Recipient)> targets)
  {
    var results = new List<string>();
    foreach (var (channel, recipient) in targets)
    {
      // VIOLACIÓN: Reutiliza la misma lógica if-else frágil
      results.Add(Send(channel, recipient, message));
    }
    return results;
  }
}
