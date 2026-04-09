using FluentAssertions;
using SolidPrinciples.LSP.Solution.ShapeHierarchy;
using Xunit;

namespace SolidPrinciples.LSP.Tests.Solution.ShapeHierarchy;

/// <summary>
/// Tests demonstrating the correct solution using IShape interface.
/// </summary>
public sealed class ShapeHierarchySolutionTests
{
    [Fact]
    public void Rectangle_HasIndependentDimensions()
    {
        // Arrange & Act
        var rectangle = new Rectangle(5, 10);

        // Assert: Properties are independent and immutable
        rectangle.Width.Should().Be(5);
        rectangle.Height.Should().Be(10);
        rectangle.CalculateArea().Should().Be(50);
    }

    [Fact]
    public void Square_HasSingleDimension()
    {
        // Arrange & Act
        var square = new Square(7);

        // Assert: Only one dimension (Side), no coupling
        square.Side.Should().Be(7);
        square.CalculateArea().Should().Be(49);
    }

    [Fact]
    public void Rectangle_ThrowsOnInvalidDimensions()
    {
        // Act & Assert: Validation at construction
        var actWidth = () => new Rectangle(0, 10);
        var actHeight = () => new Rectangle(10, 0);

        actWidth.Should().Throw<ArgumentException>()
            .WithMessage("*Width must be positive*");

        actHeight.Should().Throw<ArgumentException>()
            .WithMessage("*Height must be positive*");
    }

    [Fact]
    public void Square_ThrowsOnInvalidDimension()
    {
        // Act & Assert
        var act = () => new Square(-5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Side must be positive*");
    }

    [Fact]
    public void GeometryService_CalculatesAreaForAnyShape_NoTypeChecking()
    {
        // Arrange
        var service = new GeometryService();
        IShape rectangle = new Rectangle(4, 6);
        IShape square = new Square(5);

        // Act: Método único maneja cualquier IShape
        int rectArea = service.CalculateArea(rectangle);
        int squareArea = service.CalculateArea(square);

        // Assert: ✓ Funciona polimórficamente, no se necesita verificación de tipo
        rectArea.Should().Be(24);
        squareArea.Should().Be(25);
    }

    [Fact]
    public void GeometryService_CalculatesMultipleAreas_Polymorphically()
    {
        // Arrange
        var service = new GeometryService();
        var shapes = new List<IShape>
        {
            new Rectangle(3, 4),
            new Square(6),
            new Rectangle(2, 10),
            new Square(8)
        };

        // Act
        var areas = service.CalculateAreas(shapes);

        // Assert: Todas calculadas correctamente
        areas.Should().Equal(12, 36, 20, 64);
    }

    [Fact]
    public void GeometryService_CalculatesTotalArea()
    {
        // Arrange
        var service = new GeometryService();
        var shapes = new List<IShape>
        {
            new Rectangle(10, 5), // 50
            new Square(4),        // 16
            new Rectangle(3, 7)   // 21
        };

        // Act
        int totalArea = service.CalculateTotalArea(shapes);

        // Assert
        totalArea.Should().Be(87); // 50 + 16 + 21
    }

    [Fact]
    public void GeometryService_FindsLargestShape()
    {
        // Arrange
        var service = new GeometryService();
        var rectangle1 = new Rectangle(5, 5);
        var square1 = new Square(8);
        var rectangle2 = new Rectangle(10, 3);

        var shapes = new List<IShape> { rectangle1, square1, rectangle2 };

        // Act
        var largest = service.FindLargest(shapes);

        // Assert: Square with area 64 is largest
        largest.Should().Be(square1);
        largest!.CalculateArea().Should().Be(64);
    }

    [Fact]
    public void ShapeFactory_CreatesAppropriateShapes()
    {
        // Act
        var rectangle = ShapeFactory.CreateRectangle(4, 7);
        var square = ShapeFactory.CreateSquare(5);

        // Assert
        rectangle.Should().BeOfType<Rectangle>();
        rectangle.CalculateArea().Should().Be(28);

        square.Should().BeOfType<Square>();
        square.CalculateArea().Should().Be(25);
    }

    [Fact]
    public void ShapeFactory_CreatesSquareWhenDimensionsEqual()
    {
        // Act: Crear "forma" con dimensiones iguales
        var shape = ShapeFactory.CreateShape(6, 6);

        // Assert: La fábrica crea Square
        shape.Should().BeOfType<Square>();
        shape.CalculateArea().Should().Be(36);
    }

    [Fact]
    public void ShapeFactory_CreatesRectangleWhenDimensionsDiffer()
    {
        // Act: Crear "forma" con dimensiones diferentes
        var shape = ShapeFactory.CreateShape(4, 9);

        // Assert: La fábrica crea Rectangle
        shape.Should().BeOfType<Rectangle>();
        shape.CalculateArea().Should().Be(36);
    }

    [Fact]
    public void ColoredShape_WrapsAnyShape_CompositionPattern()
    {
        // Arrange
        IShape rectangle = new Rectangle(5, 8);
        IShape square = new Square(6);

        // Act: Componer comportamiento en lugar de heredar
        var redRectangle = new ColoredShape(rectangle, "Red");
        var blueSquare = new ColoredShape(square, "Blue");

        // Assert: La composición preserva el cálculo del área
        redRectangle.CalculateArea().Should().Be(40);
        redRectangle.Color.Should().Be("Red");
        redRectangle.Describe().Should().Be("Red shape with area 40");

        blueSquare.CalculateArea().Should().Be(36);
        blueSquare.Color.Should().Be("Blue");
        blueSquare.Describe().Should().Be("Blue shape with area 36");
    }

    [Fact]
    public void ShapeCollection_WorksWithAnyIShape_NoTypeChecking()
    {
        // Arrange
        var collection = new ShapeCollection();

        // Act: Agregar diferentes tipos de formas
        collection.Add(new Rectangle(4, 5));
        collection.Add(new Square(3));
        collection.Add(new Rectangle(2, 10));
        collection.Add(new ColoredShape(new Square(7), "Green"));

        // Assert: Todas las formas funcionan uniformemente
        collection.Shapes.Should().HaveCount(4);
        collection.GetTotalArea().Should().Be(98); // 20 + 9 + 20 + 49 = 98
    }

    [Fact]
    public void ShapeCollection_FiltersShapesByArea()
    {
        // Arrange
        var collection = new ShapeCollection();
        collection.Add(new Rectangle(2, 3));  // Area = 6
        collection.Add(new Square(5));        // Area = 25
        collection.Add(new Rectangle(4, 8));  // Area = 32
        collection.Add(new Square(2));        // Area = 4

        // Act: Obtener formas con área entre 5 y 30
        var filtered = collection.GetShapesInAreaRange(5, 30);

        // Assert: Should include 6 and 25, exclude 32 and 4
        filtered.Should().HaveCount(2);
        filtered.Select(s => s.CalculateArea()).Should().Equal(6, 25);
    }

    [Fact]
    public void PolymorphicCollection_AllShapesWorkCorrectly_NoLSPViolation()
    {
        // This test proves LSP compliance: any IShape can be substituted

        // Arrange: Heterogeneous collection
        List<IShape> shapes = new()
        {
            new Rectangle(3, 7),
            new Square(4),
            new Rectangle(10, 2),
            new Square(5),
            new ColoredShape(new Rectangle(6, 3), "Yellow")
        };

        // Act: Process all shapes uniformly
        var areas = shapes.Select(s => s.CalculateArea()).ToList();

        // Assert: ✓ All work correctly, no special cases needed
        areas.Should().Equal(21, 16, 20, 25, 18);

        // ✓ No type checking
        // ✓ No unexpected side effects
        // ✓ Polymorphism works as expected
    }

    [Fact]
    public void NoTypeCheckingNeeded_AllShapesInterchangeable()
    {
        // This test demonstrates TRUE polymorphism - no type checking

        // Arrange
        IShape shape1 = new Rectangle(8, 3);
        IShape shape2 = new Square(6);
        IShape shape3 = new ColoredShape(new Square(4), "Orange");

        // Act: ALL shapes work with the same interface
        int area1 = shape1.CalculateArea();
        int area2 = shape2.CalculateArea();
        int area3 = shape3.CalculateArea();

        // Assert: No type checking, no casting, just polymorphism
        area1.Should().Be(24);
        area2.Should().Be(36);
        area3.Should().Be(16);

        // ✓ This is what LSP looks like in practice
    }

    [Fact]
    public void Immutability_PreventsPropertyCouplingIssues()
    {
        // Arrange & Act
        var rectangle = new Rectangle(5, 10);
        var square = new Square(7);

        // Assert: Properties are immutable - can't be changed after construction
        // This prevents the coupling issues seen in the Problem example
        rectangle.Width.Should().Be(5);
        rectangle.Height.Should().Be(10);
        square.Side.Should().Be(7);

        // No setters available - no way to create inconsistent state
    }
}
