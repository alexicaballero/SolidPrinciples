using FluentAssertions;
using SolidPrinciples.ISP.Solution.PrinterExample;

namespace SolidPrinciples.ISP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Estos tests demuestran los beneficios de ISP con interfaces segregadas.
/// 
/// Beneficios:
/// - Interfaces pequeñas y enfocadas
/// - Sin NotSupportedException
/// - API confiable - si implementa la interfaz, funciona
/// - Clientes dependen solo de lo que necesitan
/// </summary>
public sealed class PrinterExampleTests
{
    [Fact]
    public void BasicInkjetPrinter_ImplementsOnlyIPrinter()
    {
        // SOLUCIÓN: BasicInkjetPrinter solo implementa lo que soporta
        IPrinter printer = new BasicInkjetPrinter();
        var doc = new Document("test.pdf", "Test content");

        // Act - Solo tiene un método y funciona
        var act = () => printer.Print(doc);

        // Assert
        act.Should().NotThrow();

        // ✓ Sin métodos muertos que lanzan excepciones
        // ✓ La interfaz refleja las capacidades reales
    }

    [Fact]
    public void OfficePrinter_ImplementsMultipleInterfaces()
    {
        // SOLUCIÓN: OfficePrinter tiene todas las capacidades,
        // así que implementa todas las interfaces

        // Arrange
        var printer = new OfficePrinter();
        var doc = new Document("report.pdf", "Report content");

        // Act & Assert - Todas las capacidades funcionan
        var printAct = () => (printer as IPrinter).Print(doc);
        var scanAct = () => (printer as IScanner).Scan(doc);
        var faxAct = () => (printer as IFax).Fax(doc);

        printAct.Should().NotThrow();
        scanAct.Should().NotThrow();
        faxAct.Should().NotThrow();

        // ✓ Sin NotSupportedException
        // ✓ Cada interfaz promete lo que realmente hace
    }

    [Fact]
    public void DocumentService_DependsOnlyOnIPrinter()
    {
        // SOLUCIÓN: Los clientes dependen solo de lo que necesitan
        IPrinter printer = new BasicInkjetPrinter();
        var service = new DocumentService(printer);
        var doc = new Document("contract.pdf", "Contract content");

        // Act
        var act = () => service.PrintDocument(doc);

        // Assert
        act.Should().NotThrow();

        // ✓ DocumentService solo necesita imprimir, solo depende de IPrinter
        // ✓ Funciona con BasicInkjetPrinter (solo imprime)
        // ✓ Sin riesgo de NotSupportedException
    }

    [Fact]
    public void DocumentService_WorksWithOfficePrinter()
    {
        // SOLUCIÓN: DocumentService funciona con cualquier IPrinter
        // aunque el dispositivo tenga más capacidades

        // Arrange
        IPrinter printer = new OfficePrinter(); // Puede hacer más
        var service = new DocumentService(printer);
        var doc = new Document("invoice.pdf", "Invoice content");

        // Act
        var act = () => service.PrintDocument(doc);

        // Assert
        act.Should().NotThrow();

        // ✓ ISP en acción: el cliente usa solo lo que necesita (Print)
        // ✓ OfficePrinter puede escanear/faxear, pero DocumentService no lo usa
        // ✓ Acoplamiento mínimo
    }

    [Fact]
    public void SegregatedInterfaces_AreSmallAndFocused()
    {
        // SOLUCIÓN: Cada interfaz tiene una sola responsabilidad

        // Assert - IPrinter tiene solo 1 método
        var printerMethods = typeof(IPrinter).GetMethods();
        printerMethods.Should().HaveCount(1);

        // Assert - IScanner tiene solo 1 método
        var scannerMethods = typeof(IScanner).GetMethods();
        scannerMethods.Should().HaveCount(1);

        // Assert - IFax tiene solo 1 método
        var faxMethods = typeof(IFax).GetMethods();
        faxMethods.Should().HaveCount(1);

        // ✓ Total: 3 interfaces enfocadas vs 1 interfaz gorda con 5 métodos
        // ✓ Cada interfaz representa una capacidad específica
        // ✓ Las clases implementan solo las interfaces que realmente soportan
    }

    [Fact]
    public void BasicInkjetPrinter_CanBeUsedAsIPrinter_Safely()
    {
        // SOLUCIÓN: El tipo en el contrato garantiza las capacidades

        // Arrange
        IPrinter printer = new BasicInkjetPrinter();
        var doc = new Document("manual.pdf", "Manual content");

        // Assert - si tengo IPrinter, Print() está garantizado
        printer.Should().NotBeNull();

        // Act - siempre seguro llamar
        printer.Print(doc);

        // ✓ Sin verificación de tipos necesaria
        // ✓ Sin try-catch defensivo
        // ✓ Si compila, funciona
    }

    [Fact]
    public void ClientCode_CanTrustInterfaces_NoExceptions()
    {
        // SOLUCIÓN: Las interfaces segregadas son confiables
        // si un objeto implementa la interfaz, el método funciona

        // Arrange
        var basicPrinter = new BasicInkjetPrinter();
        var officePrinter = new OfficePrinter();

        // Act - usar como IPrinter siempre es seguro
        var printers = new List<IPrinter> { basicPrinter, officePrinter };

        foreach (var printer in printers)
        {
            var act = () => printer.Print(new Document("test.pdf", "Test content"));
            act.Should().NotThrow();
        }

        // ✓ Polimorfismo seguro: cualquier IPrinter puede Print()
        // ✓ Sin excepciones en tiempo de ejecución
        // ✓ ISP cumplido: interfaz = capacidad garantizada
    }

    [Fact]
    public void MockingSegregatedInterfaces_IsSimple()
    {
        // SOLUCIÓN: Mockear interfaces pequeñas es trivial

        // Arrange - solo necesito mockear IPrinter (1 método)
        // vs IMultiFunctionDevice (5 métodos en Problem)
        var printerMock = new PrinterMock();
        var service = new DocumentService(printerMock);

        // Act
        service.PrintDocument(new Document("test.pdf", "Test content"));

        // Assert
        printerMock.WasCalled.Should().BeTrue();

        // ✓ Mock simple: solo 1 método
        // ✓ Test enfocado: verifica solo lo que importa
    }

    // Mock simple para demostración
    private sealed class PrinterMock : IPrinter
    {
        public bool WasCalled { get; private set; }

        public void Print(Document doc)
        {
            WasCalled = true;
        }
    }
}
