namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Interfaz remitente — responsable SÓLO de entregar un informe.
/// Cambiar el canal de entrega (correo → Slack) requiere una nueva implementación, no modificar el código existente.
/// </summary>
public interface IReportSender
{
  Task SendAsync(string recipient, string subject, string content, CancellationToken ct = default);
}
