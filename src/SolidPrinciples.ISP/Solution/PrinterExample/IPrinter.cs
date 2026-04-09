namespace SolidPrinciples.ISP.Solution.PrinterExample;

/// <summary>
/// SOLUCIÓN ISP: Interfaz enfocada solo en impresión.
/// </summary>
/// <remarks>
/// Esta interfaz define SOLO la capacidad de imprimir.
/// - Cualquier dispositivo que implemente IPrinter puede imprimir
/// - No promete capacidades que no tiene
/// - Los clientes que necesitan solo impresión dependen solo de esto
///
/// Beneficio clave: Cada clase implementa solo lo que realmente soporta.
/// </remarks>
public interface IPrinter
{
    void Print(Document doc);
}
