namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEM: Concrete file system logger.
/// High-level CommunityService depends on this directly.
/// </summary>
/// <remarks>
/// This class is low-level infrastructure (file I/O). It should be behind an abstraction.
/// </remarks>
public sealed class FileSystemLogger
{
    /// <summary>
    /// Logs a message to a file.
    /// CommunityService directly depends on this file system-specific method.
    /// </summary>
    public void Log(string message)
    {
        // Simulated file write: File.AppendAllText("log.txt", message)
        Console.WriteLine($"[FILE LOG] {message}");
    }
}
