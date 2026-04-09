namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Remitente de notificación de consola para pruebas.
/// Implementa la misma abstracción que EmailNotificationSender.
/// Se puede inyectar en CreateSessionHandler para pruebas/desarrollo.
/// </summary>
public sealed class ConsoleNotificationSender : INotificationSender
{
    public Task SendAsync(string recipient, string message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[CONSOLE] To {recipient}: {message}");
        return Task.CompletedTask;
    }
}
