namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Implementación concreta de registrador del sistema de archivos.
/// Esta clase implementa la abstracción ILogger.
/// El código de alto nivel NO importa esta clase directamente.
/// </summary>
public sealed class FileSystemLogger : ILogger
{
    public void Log(string message)
    {
        // Lógica de I/O de archivo: File.AppendAllText("log.txt", $"{DateTime.Now}: {message}\n");
        Console.WriteLine($"[FILE LOG] {message}");
    }
}
