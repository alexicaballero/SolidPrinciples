using FluentAssertions;
using SolidPrinciples.ISP.Problem.PrinterExample;

namespace SolidPrinciples.ISP.Tests.Problem;

/// <summary>
/// PROBLEMA: Estos tests demuestran por qué las interfaces gordas violan ISP.
/// 
/// Puntos de dolor:
/// - Implementaciones que lanzan NotSupportedException
/// - Métodos que compilan pero fallan en tiempo de ejecución
/// - API poco confiable - no sabes qué métodos funcionan
/// </summary>
public sealed class PrinterExampleTests
{
    [Fact]
    public void BasicInkjetPrinter_Print_Works()
    {
        // Arrange
        IMultiFunctionDevice printer = new BasicInkjetPrinter();
        var doc = new Document("test.pdf", "Test content");

        // Act - Este método funciona
        var act = () => printer.Print(doc);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void BasicInkjetPrinter_Scan_ThrowsNotSupportedException()
    {
        // PROBLEMA: Compila pero falla en tiempo de ejecución
        IMultiFunctionDevice printer = new BasicInkjetPrinter();
        var doc = new Document("test.pdf", "Test content");

        // Act
        var act = () => printer.Scan(doc);

        // Assert - PROBLEMA: Excepción en tiempo de ejecución
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*no puede escanear*");
    }

    [Fact]
    public void BasicInkjetPrinter_Fax_ThrowsNotSupportedException()
    {
        // PROBLEMA: Compila pero falla en tiempo de ejecución
        IMultiFunctionDevice printer = new BasicInkjetPrinter();
        var doc = new Document("test.pdf", "Test content");

        // Act
        var act = () => printer.Fax(doc);

        // Assert - PROBLEMA: Excepción en tiempo de ejecución
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*no puede enviar fax*");
    }

    [Fact]
    public void BasicInkjetPrinter_Staple_ThrowsNotSupportedException()
    {
        // PROBLEMA: Compila pero falla en tiempo de ejecución
        IMultiFunctionDevice printer = new BasicInkjetPrinter();
        var doc = new Document("test.pdf", "Test content");

        // Act
        var act = () => printer.Staple(doc);

        // Assert - PROBLEMA: Excepción en tiempo de ejecución
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*no puede engrapar*");
    }

    [Fact]
    public void BasicInkjetPrinter_PhotoCopy_ThrowsNotSupportedException()
    {
        // PROBLEMA: Compila pero falla en tiempo de ejecución
        IMultiFunctionDevice printer = new BasicInkjetPrinter();
        var doc = new Document("test.pdf", "Test content");

        // Act
        var act = () => printer.PhotoCopy(doc);

        // Assert - PROBLEMA: Excepción en tiempo de ejecución
        act.Should().Throw<NotSupportedException>()
            .WithMessage("*no puede fotocopiar*");
    }

    [Fact]
    public void FatInterface_ForcesImplementationOf5Methods_ButOnly1Works()
    {
        // PROBLEMA: IMultiFunctionDevice obliga a implementar 5 métodos
        // pero BasicInkjetPrinter solo soporta 1 (Print)

        // Arrange
        IMultiFunctionDevice printer = new BasicInkjetPrinter();

        // Assert - la interfaz promete 5 capacidades
        var methods = typeof(IMultiFunctionDevice).GetMethods();
        methods.Should().HaveCount(5);

        // PROBLEMA: 4 de 5 métodos lanzan excepciones
        // La interfaz miente sobre las capacidades reales
        // ✗ Print()     → ✓ Funciona
        // ✗ Scan()      → ✗ NotSupportedException
        // ✗ Fax()       → ✗ NotSupportedException
        // ✗ Staple()    → ✗ NotSupportedException
        // ✗ PhotoCopy() → ✗ NotSupportedException
    }

    [Fact]
    public void ClientCode_CannotTrustInterface_MustKnowImplementation()
    {
        // PROBLEMA: El código cliente que recibe IMultiFunctionDevice
        // no puede confiar en que todos los métodos funcionen

        // Arrange
        IMultiFunctionDevice printer = new BasicInkjetPrinter();
        var doc = new Document("report.pdf", "Report content");

        // El cliente no sabe qué métodos son seguros
        // Debe conocer la implementación específica o capturar excepciones

        // PROBLEMA: Este código compila pero puede fallar
        var canScan = CanScanSafely(printer, doc);
        canScan.Should().BeFalse();

        // PROBLEMA: Necesitas defensivo try-catch o verificación de tipos
    }

    private bool CanScanSafely(IMultiFunctionDevice device, Document doc)
    {
        try
        {
            device.Scan(doc);
            return true;
        }
        catch (NotSupportedException)
        {
            // PROBLEMA: Usar excepciones para flujo de control
            return false;
        }
    }
}
