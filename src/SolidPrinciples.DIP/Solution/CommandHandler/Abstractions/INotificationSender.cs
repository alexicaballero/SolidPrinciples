namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// SOLUCIÓN: Abstracción para envío de notificaciones.
/// El código de alto nivel depende de esta interfaz, no de implementaciones concretas.
/// </summary>
public interface INotificationSender
{
    Task SendAsync(string recipient, string message, CancellationToken cancellationToken);
}
