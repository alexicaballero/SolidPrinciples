namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Registrador de consola para pruebas/desarrollo.
/// Implementa ILogger, se puede inyectar en CommunityService.
/// </summary>
public sealed class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[CONSOLE LOG] {message}");
    }
}
