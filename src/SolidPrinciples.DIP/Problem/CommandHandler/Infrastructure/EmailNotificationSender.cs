namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEMA: Implementación concreta de remitente de correo con lógica específica de SMTP.
/// El código de alto nivel depende directamente de esto, haciendo imposible intercambiar implementaciones.
/// </summary>
/// <remarks>
/// Esta clase es infraestructura de bajo nivel. La lógica empresarial NO debería depender de esto directamente.
/// </remarks>
public sealed class EmailNotificationSender
{
    /// <summary>
    /// Envía un correo electrónico vía SMTP.
    /// El código de alto nivel depende de este método concreto.
    /// </summary>
    public async Task SendAsync(string recipient, string message, CancellationToken cancellationToken)
    {
        // Simulated SMTP email sending
        // In a real system: using System.Net.Mail.SmtpClient
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"[EMAIL] Sent to {recipient}: {message}");
    }
}
