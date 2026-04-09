namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// SOLUCIÓN: Abstracción para registro.
/// El código de alto nivel depende de esta interfaz.
/// </summary>
public interface ILogger
{
    void Log(string message);
}
