namespace SolidPrinciples.ISP.Solution.PrinterExample;

/// <summary>
/// SOLUCIÓN ISP: Interfaz enfocada solo en escaneo.
/// </summary>
/// <remarks>
/// Cada capacidad es su propia interfaz.
/// Solo los dispositivos que pueden escanear implementan IScanner.
/// </remarks>
public interface IScanner
{
    void Scan(Document doc);
}
