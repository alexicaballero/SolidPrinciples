namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// SOLUCIÓN: Abstracción para un canal de notificación.
/// Los nuevos canales se agregan creando nuevas implementaciones — sin necesidad de modificaciones.
/// </summary>
public interface INotificationChannel
{
  /// <summary>
  /// El identificador del canal (p. ej., "email", "sms", "slack").
  /// </summary>
  string Channel { get; }

  /// <summary>
  /// Envía una notificación a través de este canal.
  /// </summary>
  string Send(string recipient, string message);
}
