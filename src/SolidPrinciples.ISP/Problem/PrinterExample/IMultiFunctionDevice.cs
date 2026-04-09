namespace SolidPrinciples.ISP.Problem.PrinterExample;

/// <summary>
/// VIOLACIÓN ISP: Una interfaz para gobernarlos a todos.
/// </summary>
/// <remarks>
/// Esta interfaz gorda obliga a todas las impresoras a implementar TODAS las capacidades,
/// incluso si solo soportan algunas de ellas.
///
/// Problema:
/// - BasicInkjetPrinter solo imprime, pero debe implementar 5 métodos
/// - Los métodos no soportados lanzan NotSupportedException
/// - Los clientes no pueden confiar en que todos los métodos funcionen
/// - Cualquier cliente que reciba IMultiFunctionDevice no sabe qué métodos son seguros
///
/// Impacto:
/// - Excepciones en tiempo de ejecución en lugar de errores de compilación
/// - API poco confiable y confusa
/// - Pruebas complicadas debido a métodos no implementados
/// </remarks>
public interface IMultiFunctionDevice
{
    void Print(Document doc);
    void Scan(Document doc);
    void Fax(Document doc);
    void Staple(Document doc);
    void PhotoCopy(Document doc);
}
