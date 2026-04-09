namespace SolidPrinciples.ISP.Solution.PrinterExample;

/// <summary>
/// SOLUCIÓN ISP: Interfaz enfocada solo en fax.
/// </summary>
/// <remarks>
/// Segregación de interfaces: cada capacidad es independiente.
/// Solo los dispositivos que pueden enviar fax implementan IFax.
/// </remarks>
public interface IFax
{
    void Fax(Document doc);
}
