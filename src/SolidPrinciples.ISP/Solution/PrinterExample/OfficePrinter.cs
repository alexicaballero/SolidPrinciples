namespace SolidPrinciples.ISP.Solution.PrinterExample;

/// <summary>
/// CORRECTO: El dispositivo multifunción implementa múltiples interfaces.
/// </summary>
/// <remarks>
/// OfficePrinter tiene todas las capacidades, así que implementa todas las interfaces.
///
/// Patrón clave: Composición de interfaces
/// - Cada interfaz representa una capacidad
/// - Las clases implementan las interfaces que realmente soportan
/// - No hay jerarquía de herencia forzada
///
/// Ventajas:
/// ✓ Flexibilidad: nuevos dispositivos eligen qué interfaces implementar
/// ✓ Sin métodos falsos
/// ✓ Los clientes dependen solo de las capacidades que necesitan
/// </remarks>
public sealed class OfficePrinter : IPrinter, IScanner, IFax
{
    public void Print(Document doc)
    {
        Console.WriteLine($"OfficePrinter imprimiendo: {doc.Name}");
    }

    public void Scan(Document doc)
    {
        Console.WriteLine($"OfficePrinter escaneando: {doc.Name}");
    }

    public void Fax(Document doc)
    {
        Console.WriteLine($"OfficePrinter enviando fax: {doc.Name}");
    }
}
