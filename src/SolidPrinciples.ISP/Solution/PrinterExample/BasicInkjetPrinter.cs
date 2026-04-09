namespace SolidPrinciples.ISP.Solution.PrinterExample;

/// <summary>
/// CORRECTO: La impresora básica implementa solo lo que soporta.
/// </summary>
/// <remarks>
/// Con ISP aplicado, BasicInkjetPrinter:
/// ✓ Implementa solo IPrinter (1 método)
/// ✓ No tiene métodos muertos que lanzan excepciones
/// ✓ Es honesta sobre sus capacidades
/// ✓ Puede ser sustituida en cualquier lugar que acepte IPrinter
///
/// Beneficios:
/// - Sin NotSupportedException
/// - Signature refleja capacidades reales
/// - Clientes seguros: si tienen IPrinter, Print() funciona
/// - Pruebas simples: solo 1 método para mockear
/// </remarks>
public sealed class BasicInkjetPrinter : IPrinter
{
    public void Print(Document doc)
    {
        Console.WriteLine($"Imprimiendo: {doc.Name}");
    }

    // Eso es todo. Sin métodos no soportados.
}
