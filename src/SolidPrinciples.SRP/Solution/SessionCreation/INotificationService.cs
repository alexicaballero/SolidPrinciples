namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Interfaz de notificación — abstrae la entrega de notificaciones como su propia responsabilidad.
/// Ya sea correo electrónico, Slack o SMS se decide en otro lugar — no en la lógica comercial.
/// </summary>
public interface INotificationService
{
  Task SendSessionCreatedAsync(Guid sessionId, string title, string speaker, CancellationToken ct = default);
}
