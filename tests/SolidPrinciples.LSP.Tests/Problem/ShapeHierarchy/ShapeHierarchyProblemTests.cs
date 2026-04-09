using FluentAssertions;
using SolidPrinciples.LSP.Problem.ShapeHierarchy;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Problem.ShapeHierarchy;

/// <summary>
/// Tests demonstrating the classic Rectangle/Square LSP violation.
/// </summary>
public sealed class ShapeHierarchyProblemTests
{
    [Fact]
    public void Rectangle_SettingWidthAndHeight_PropertiesAreIndependent()
    {
        // Arrange
        var rectangle = new Rectangle();

        // Act
        rectangle.Width = 5;
        rectangle.Height = 10;

        // Assert: Properties are independent (as expected)
        rectangle.Width.Should().Be(5);
        rectangle.Height.Should().Be(10);
        rectangle.CalculateArea().Should().Be(50);
    }

    [Fact]
    public void Square_SettingWidth_AlsoChangesHeight_ViolatesLSP()
    {
        // Arrange
        var square = new Square();

        // Act
        square.Width = 5;
        square.Height = 10; // This will overwrite Width to 10!

        // Assert: HIDDEN SIDE EFFECT - Width changed unexpectedly
        square.Width.Should().Be(10); // Not 5 as set earlier!
        square.Height.Should().Be(10);
        square.CalculateArea().Should().Be(100); // Not 50!
    }

    [Fact]
    public void Square_SettingHeight_AlsoChangesWidth_ViolatesLSP()
    {
        // Arrange
        var square = new Square();

        // Act
        square.Height = 7;
        square.Width = 3; // This will overwrite Height to 3!

        // Assert: EFECTO SECUNDARIO OCULTO - Height cambió inesperadamente
        square.Width.Should().Be(3);
        square.Height.Should().Be(3); // ¡No 7 como se estableció antes!
        square.CalculateArea().Should().Be(9); // ¡No 21!
    }

    [Fact]
    public void GeometryService_ResizeAndVerify_WorksForRectangle()
    {
        // Arrange
        var service = new GeometryService();
        var rectangle = new Rectangle();

        // Act
        service.ResizeAndVerify(rectangle);

        // Assert: Rectangle funciona como se espera
        rectangle.Width.Should().Be(5);
        rectangle.Height.Should().Be(10);
        rectangle.CalculateArea().Should().Be(50);
    }

    [Fact]
    public void GeometryService_ResizeAndVerify_FailsForSquare_DemonstratesLSPViolation()
    {
        // Arrange
        var service = new GeometryService();
        var square = new Square();

        // Act
        service.ResizeAndVerify(square);

        // Assert: Square produce resultado INCORRECTO
        // Esperado Width=5, Height=10, Area=50
        // Real: Width=10, Height=10, Area=100
        square.Width.Should().Be(10); // El último setter ganó
        square.Height.Should().Be(10);
        square.CalculateArea().Should().Be(100); // ¡Incorrecto! Debería ser 50
    }

    [Fact]
    public void GeometryService_IsLandscape_FailsForSquare()
    {
        // Arrange
        var service = new GeometryService();
        var rectangle = new Rectangle();
        var square = new Square();

        // Act
        bool rectangleIsLandscape = service.IsLandscape(rectangle);
        bool squareIsLandscape = service.IsLandscape(square);

        // Assert
        rectangleIsLandscape.Should().BeTrue(); // 20 > 10 ✓
        squareIsLandscape.Should().BeFalse(); // ¡Ambos 10! Se esperaba true
    }

    [Fact]
    public void ShapeBatchProcessor_CalculateAreas_ProducesIncorrectResultsWithSquare()
    {
        // Arrange
        var processor = new ShapeBatchProcessor();
        var shapes = new List<Rectangle>
        {
            new Rectangle(), // Rectangle
            new Square(),    // Square (violates LSP)
            new Rectangle()  // Rectangle
        };

        // Act
        var areas = processor.CalculateAreas(shapes);

        // Assert: Resultados mixtos debido a violación de LSP
        areas[0].Should().Be(24); // Rectangle: 4 * 6 = 24 ✓
        areas[1].Should().Be(36); // Square: 6 * 6 = 36 Se esperaba 24
        areas[2].Should().Be(24); // Rectangle: 4 * 6 = 24 ✓
    }

    [Fact]
    public void ShapeBatchProcessor_ScaleShapes_ProducesUnexpectedResultsWithSquare()
    {
        // Arrange
        var processor = new ShapeBatchProcessor();
        var rectangle = new Rectangle { Width = 10, Height = 5 };
        var square = new Square { Width = 10, Height = 10 };

        var shapes = new List<Rectangle> { rectangle, square };

        // Act: Scale width by 2, height by 3
        processor.ScaleShapes(shapes, widthFactor: 2, heightFactor: 3);

        // Assert
        // Rectangle: Width = 20, Height = 15 ✓ Correct
        rectangle.Width.Should().Be(20);
        rectangle.Height.Should().Be(15);

        // Square: The scaling is applied twice due to coupling
        // Width *= 2 -> sets both to 20
        // Height *= 3 -> multiplies current value (20) by 3 = 60
        square.Width.Should().Be(60); // Both end up 60
        square.Height.Should().Be(60);
    }

    [Fact]
    public void PolymorphicCollection_TreatSquareAsRectangle_ProducesInconsistentBehavior()
    {
        // This test demonstrates that you CANNOT safely substitute Square for Rectangle

        // Arrange: Collection of "rectangles"
        List<Rectangle> rectangles = new()
        {
            new Rectangle { Width = 4, Height = 6 },
            new Square { Width = 5, Height = 5 },
            new Rectangle { Width = 3, Height = 8 }
        };

        // Act: Try to resize all rectangles uniformly
        foreach (var rect in rectangles)
        {
            rect.Width = 10;
            rect.Height = 20;
        }

        // Assert: Only actual Rectangles behave correctly
        rectangles[0].Width.Should().Be(10);
        rectangles[0].Height.Should().Be(20);
        rectangles[0].CalculateArea().Should().Be(200); // ✓

        rectangles[1].Width.Should().Be(20); // Not 10! Side effect
        rectangles[1].Height.Should().Be(20);
        rectangles[1].CalculateArea().Should().Be(400); // Wrong!

        rectangles[2].Width.Should().Be(10);
        rectangles[2].Height.Should().Be(20);
        rectangles[2].CalculateArea().Should().Be(200); // ✓
    }

    [Fact]
    public void TypeChecking_CodeSmell_RequiredDueToLSPViolation()
    {
        // Arrange
        var service = new GeometryService();
        Rectangle rect = new Rectangle { Width = 5, Height = 10 };
        Rectangle square = new Square { Width = 5, Height = 5 };

        // Act: Use "safe" method that checks types
        int rectArea = service.CalculateAreaSafely(rect);
        int squareArea = service.CalculateAreaSafely(square);

        // Assert: Type checking allows it to work, but it's a code smell
        rectArea.Should().Be(50);
        squareArea.Should().Be(25); // Square: 5 * 5 = 25

        // This is a symptom that Square violates LSP
        // If substitution worked correctly, we wouldn't need CalculateAreaSafely
    }
}
