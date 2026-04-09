namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Registrador de Azure Application Insights para producción.
/// Implementa ILogger, demuestra el intercambio de implementaciones.
/// </summary>
/// <remarks>
/// Registrado en contenedor DI para producción:
/// services.AddSingleton&lt;ILogger, AzureAppInsightsLogger&gt;();
///
/// CommunityService no cambia cuando intercambiamos de FileSystemLogger
/// a AzureAppInsightsLogger. Este es el poder de DIP.
/// </remarks>
public sealed class AzureAppInsightsLogger : ILogger
{
    public void Log(string message)
    {
        // Lógica de Azure Application Insights: _telemetryClient.TrackTrace(message);
        Console.WriteLine($"[AZURE AI] {message}");
    }
}
