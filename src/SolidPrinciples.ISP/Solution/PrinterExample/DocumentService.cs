namespace SolidPrinciples.ISP.Solution.PrinterExample;

/// <summary>
/// CORRECTO: Los clientes dependen SOLO de lo que necesitan.
/// </summary>
/// <remarks>
/// DocumentService solo necesita imprimir, así que depende solo de IPrinter.
///
/// Beneficios de ISP en acción:
/// ✓ Funciona con BasicInkjetPrinter (solo imprime)
/// ✓ Funciona con OfficePrinter (puede hacer más, pero usamos solo Print)
/// ✓ Sin riesgo de NotSupportedException
/// ✓ Dependency Injection precisa: inyecta solo lo necesario
/// ✓ Pruebas simples: mockear IPrinter = 1 método
///
/// Contraste con el problema:
/// Antes: dependía de IMultiFunctionDevice (5 métodos)
/// ✓ Ahora: depende de IPrinter (1 método)
/// </remarks>
public sealed class DocumentService
{
    private readonly IPrinter _printer;

    public DocumentService(IPrinter printer)
    {
        // Esto funciona con BasicInkjetPrinter Y OfficePrinter
        // Sin riesgo de NotSupportedException
        _printer = printer;
    }

    public void PrintDocument(Document doc)
    {
        // Siempre seguro - si tenemos IPrinter, Print() funciona
        _printer.Print(doc);
    }
}
