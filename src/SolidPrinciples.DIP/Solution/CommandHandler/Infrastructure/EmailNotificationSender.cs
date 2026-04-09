namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Implementación concreta de remitente de correo.
/// Esta clase depende de la abstracción INotificationSender (la implementa).
/// El código de alto nivel NO depende de esta clase directamente.
/// </summary>
public sealed class EmailNotificationSender : INotificationSender
{
    public async Task SendAsync(string recipient, string message, CancellationToken cancellationToken)
    {
        // SMTP-specific email logic
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"[EMAIL] Sent to {recipient}: {message}");
    }
}
