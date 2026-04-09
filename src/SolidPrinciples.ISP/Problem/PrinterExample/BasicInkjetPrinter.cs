namespace SolidPrinciples.ISP.Problem.PrinterExample;

/// <summary>
/// VIOLACIÓN: Una impresora básica forzada a implementar capacidades que no tiene.
/// </summary>
/// <remarks>
/// BasicInkjetPrinter solo puede imprimir, pero IMultiFunctionDevice la obliga
/// a implementar 5 métodos. Los otros 4 son trampas que lanzan excepciones en tiempo de ejecución.
///
/// Señales de advertencia:
/// - 4 de 5 métodos lanzan NotSupportedException
/// - La clase "miente" al implementar una interfaz que promete capacidades que no tiene
/// - Los clientes deben saber la implementación específica para evitar excepciones
///
/// Consecuencias:
/// - El código que recibe IMultiFunctionDevice puede fallar en tiempo de ejecución
/// - Se requiere verificación de tipos o documentación externa
/// - Viola el principio de sustitución de Liskov (LSP)
/// </remarks>
public sealed class BasicInkjetPrinter : IMultiFunctionDevice
{
    public void Print(Document doc)
    {
        // Esto funciona bien - es la única capacidad que la impresora tiene
        Console.WriteLine($"Imprimiendo: {doc.Name}");
    }

    public void Scan(Document doc)
    {
        // PROBLEMA: Compilar pero lanzar en tiempo de ejecución
        throw new NotSupportedException("Esta impresora no puede escanear");
    }

    public void Fax(Document doc)
    {
        // PROBLEMA: Compilar pero lanzar en tiempo de ejecución
        throw new NotSupportedException("Esta impresora no puede enviar fax");
    }

    public void Staple(Document doc)
    {
        // PROBLEMA: Compilar pero lanzar en tiempo de ejecución
        throw new NotSupportedException("Esta impresora no puede engrapar");
    }

    public void PhotoCopy(Document doc)
    {
        // PROBLEMA: Compilar pero lanzar en tiempo de ejecución
        throw new NotSupportedException("Esta impresora no puede fotocopiar");
    }
}
