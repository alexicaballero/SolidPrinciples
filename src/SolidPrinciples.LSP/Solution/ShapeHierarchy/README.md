# Caso de Uso: Jerarquía de Figuras (Rectángulo/Cuadrado) — Solución 📐

> **Objetivo de aprendizaje**: Entender que las relaciones matemáticas "es-un" no siempre se traducen correctamente a herencia en código. LSP nos enseña que la substituibilidad es lo que importa, no la taxonomía.

## 📚 Artículo de referencia

Esta solución implementa el diseño correcto explicado en:
**[Principio de Sustitución de Liskov - El Problema: Subtipos que Rompen Expectativas](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/liskov-substitution#el-problema-subtipos-que-rompen-expectativas)**

## 🎯 La solución: Composición con interfaz común

### El diseño correcto

```
IShape (interfaz común)
  ├─ Rectangle (Width y Height independientes)
  └─ Square (solo Side)
```

**Clave:** Rectangle y Square **NO** tienen relación de herencia entre sí.

### Cómo funciona

#### 1. IShape define solo el contrato esencial

```csharp
public interface IShape
{
    int CalculateArea();
}
```

**El contrato:**

- ✓ Todas las figuras pueden calcular su área
- ✓ Sin suposiciones sobre cómo se almacenan las dimensiones
- ✓ Sin acoplamiento entre diferentes tipos de figuras

#### 2. Rectangle: Dos dimensiones independientes

```csharp
public sealed class Rectangle : IShape
{
    public int Width { get; }
    public int Height { get; }

    public Rectangle(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        Width = width;
        Height = height;
    }

    public int CalculateArea() => Width * Height;
}
```

**Por qué esto es correcto:**

- ✓ `Width` y `Height` son **verdaderamente independientes**
- ✓ Propiedades inmutables (solo lectura) previenen inconsistencias
- ✓ El constructor garantiza un estado válido
- ✓ No hay efectos secundarios ocultos

#### 3. Square: Una sola dimensión

```csharp
public sealed class Square : IShape
{
    public int Side { get; }

    public Square(int side)
    {
        if (side <= 0)
            throw new ArgumentException("Side must be positive", nameof(side));

        Side = side;
    }

    public int CalculateArea() => Side * Side;
}
```

**Por qué esto es correcto:**

- ✓ Tiene su **propio contrato apropiado**: solo una dimensión `Side`
- ✓ No puede ser mal usado estableciendo dimensiones independientes
- ✓ Sin acoplamiento oculto ni efectos secundarios
- ✓ **Insight clave:** En código, "es-un" significa "es sustituible por", no "es un tipo de"

## 🎯 La lección fundamental

> **"En matemáticas un cuadrado ES un rectángulo. En programación, un Square NO ES un Rectangle."**

¿Por qué?

- **Matemáticas:** Relaciones taxonómicas basadas en propiedades estáticas
- **Programación:** Substituibilidad basada en comportamiento dinámico

Un `Square` NO puede sustituir a un `Rectangle` sin romper expectativas:

```csharp
// Contract: Width y Height son independientes
Rectangle rect = new Rectangle(5, 10);
rect.Width = 15;  // Esperado: Width = 15, Height = 10
// Real con Square: Width = 15, Height = 15 ✗ ROTO
```

## 🎯 Beneficios de cumplir con LSP

### 1. Código polimórfico funciona correctamente

```csharp
public sealed class GeometryService
{
    public int CalculateArea(IShape shape)
    {
        // Funciona con Rectangle, Square o cualquier IShape futuro
        // Sin verificación de tipos, sin casos especiales
        return shape.CalculateArea();
    }

    public int CalculateTotalArea(IEnumerable<IShape> shapes)
    {
        return shapes.Sum(shape => shape.CalculateArea());
    }
}
```

**Uso:**

```csharp
var service = new GeometryService();

// Todas las figuras funcionan correctamente
var rect = new Rectangle(5, 10);
var square = new Square(7);

Assert.Equal(50, service.CalculateArea(rect));   // 5 × 10 = 50 ✓
Assert.Equal(49, service.CalculateArea(square)); // 7 × 7 = 49 ✓

// Calcula área total sin conocer tipos específicos
IShape[] shapes = [rect, square, new Rectangle(3, 4)];
Assert.Equal(111, service.CalculateTotalArea(shapes)); // 50 + 49 + 12 ✓
```

### 2. Procesamiento por lotes sin casos especiales

```csharp
public sealed class ShapeBatchProcessor
{
    public IReadOnlyList<int> ProcessAreas(IEnumerable<IShape> shapes)
    {
        // ✓ Sin verificación de tipos
        // ✓ Sin lógica especial para Square
        // ✓ Todos los cálculos son correctos
        return shapes
            .Select(shape => shape.CalculateArea())
            .ToList()
            .AsReadOnly();
    }
}
```

### 3. Extensibilidad con decoradores (Patrón Decorator)

```csharp
public sealed class ColoredShape : IShape
{
    private readonly IShape _shape;
    private readonly string _color;

    public ColoredShape(IShape shape, string color)
    {
        _shape = shape;
        _color = color;
    }

    public string Color => _color;

    public int CalculateArea() => _shape.CalculateArea();

    public string Describe() => $"{_color} shape with area {CalculateArea()}";
}
```

**Uso:**

```csharp
IShape redSquare = new ColoredShape(new Square(5), "Red");
IShape blueRectangle = new ColoredShape(new Rectangle(4, 6), "Blue");

Assert.Equal(25, redSquare.CalculateArea());    // ✓ Correcto
Assert.Equal(24, blueRectangle.CalculateArea()); // ✓ Correcto
```

**Beneficio:** Puedes agregar comportamiento (color, borde, sombra) sin violar LSP ni modificar las clases base.

### 4. Colecciones heterogéneas funcionan correctamente

```csharp
public sealed class ShapeCollection
{
    private readonly List<IShape> _shapes = [];

    public void Add(IShape shape) => _shapes.Add(shape);

    public int GetTotalArea() => _shapes.Sum(s => s.CalculateArea());

    public IShape? GetLargest() =>
        _shapes.OrderByDescending(s => s.CalculateArea()).FirstOrDefault();
}
```

**Uso:**

```csharp
var collection = new ShapeCollection();
collection.Add(new Rectangle(5, 10));  // Área = 50
collection.Add(new Square(8));          // Área = 64
collection.Add(new Rectangle(3, 4));   // Área = 12

Assert.Equal(126, collection.GetTotalArea()); // ✓ Correcto
var largest = collection.GetLargest();
Assert.Equal(64, largest.CalculateArea());    // ✓ Es Square(8)
```

### 5. Las pruebas son simples y polimórficas

```csharp
[Theory]
[InlineData(5, 10, 50)]  // Rectangle
[InlineData(7, 7, 49)]   // Square (puede construirse como Rectangle si es necesario)
public void CalculateArea_ReturnsCorrectResult(int dim1, int dim2, int expected)
{
    IShape shape = dim1 == dim2
        ? new Square(dim1)
        : new Rectangle(dim1, dim2);

    Assert.Equal(expected, shape.CalculateArea());
}

[Fact]
public void AllShapes_CanBeProcessedUniformly()
{
    // Arrange: Colección heterogénea
    IShape[] shapes = [
        new Rectangle(5, 10),
        new Square(7),
        new ColoredShape(new Square(3), "Red")
    ];

    // Act: Procesa todos uniformemente
    var areas = shapes.Select(s => s.CalculateArea()).ToArray();

    // Assert: Todos calculan correctamente
    Assert.Equal([50, 49, 9], areas);
}
```

## ✅ Cómo esto previene las violaciones del problema

| Violación en Problem/             | Cómo se previene en Solution/                                            |
| --------------------------------- | ------------------------------------------------------------------------ |
| `Square` hereda de `Rectangle`    | No hay herencia - tipos independientes con interfaz común                |
| Asignar `Width` modifica `Height` | `Rectangle` tiene propiedades independientes; `Square` solo tiene `Side` |
| Expectativas rotas en cálculos    | Cada tipo tiene su propio contrato correcto                              |
| Verificación de tipos necesaria   | Todo el código funciona con `IShape` polimórficamente                    |
| Resultados incorrectos            | Todos los cálculos son matemáticamente correctos                         |

## 📖 Reglas de LSP aplicadas

| Regla                                  | Cumplimiento                                                        |
| -------------------------------------- | ------------------------------------------------------------------- |
| **Subtypes can substitute base types** | ✓ Cualquier `IShape` puede sustituir a otro en código polimórfico   |
| **No strengthened preconditions**      | ✓ Todas las figuras respetan el mismo contrato de `CalculateArea()` |
| **No weakened postconditions**         | ✓ Todas las figuras devuelven áreas correctas                       |
| **No hidden side effects**             | ✓ Las propiedades son independientes o singletons (Side)            |

## 🧪 Destacados de las pruebas

```csharp
[Fact]
public void Rectangle_DimensionsAreIndependent()
{
    var rect = new Rectangle(5, 10);

    Assert.Equal(5, rect.Width);
    Assert.Equal(10, rect.Height);
    Assert.Equal(50, rect.CalculateArea());
    // ✓ Las dimensiones permanecen independientes
}

[Fact]
public void Square_HasOneDimension()
{
    var square = new Square(7);

    Assert.Equal(7, square.Side);
    Assert.Equal(49, square.CalculateArea());
    // ✓ No hay Width/Height para mal usar
}

[Fact]
public void GeometryService_WorksWithAllShapes()
{
    var service = new GeometryService();
    IShape[] shapes = [
        new Rectangle(4, 5),
        new Square(6),
        new ColoredShape(new Rectangle(2, 3), "Blue")
    ];

    var areas = service.CalculateAreas(shapes);

    Assert.Equal([20, 36, 6], areas);
    // ✓ Sin verificación de tipos, todos correctos
}
```

## 🚀 Extensibilidad

¿Quieres agregar nuevas figuras?

```csharp
public sealed class Circle : IShape
{
    public int Radius { get; }

    public Circle(int radius)
    {
        if (radius <= 0)
            throw new ArgumentException("Radius must be positive");
        Radius = radius;
    }

    public int CalculateArea() => (int)(Math.PI * Radius * Radius);
}
```

**Sin cambios en:**

- `GeometryService` - funciona automáticamente con `Circle`
- `ShapeBatchProcessor` - procesa `Circle` como cualquier otra figura
- `ShapeCollection` - puede contener `Circle`
- Código de pruebas polimórfico

## Archivos

| Archivo              | Descripción                                          |
| -------------------- | ---------------------------------------------------- |
| `IShape.cs`          | Interfaz común para todas las figuras                |
| `Rectangle.cs`       | Implementación con `Width` y `Height` independientes |
| `Square.cs`          | Implementación con única dimensión `Side`            |
| `GeometryService.cs` | Servicio que procesa figuras polimórficamente        |
| `ShapeCollection.cs` | Colección que maneja figuras heterogéneas            |
| `ShapeFactory.cs`    | Fábrica para crear figuras                           |
| `ColoredShape.cs`    | Decorador que agrega color sin violar LSP            |

## 💡 Conclusiones clave

1. **Las relaciones matemáticas ≠ relaciones de código** — Un cuadrado matemático es un rectángulo; un `Square` en código NO es un `Rectangle`
2. **"Es-un" significa "es sustituible por"** — No taxonomía, sino comportamiento compatible
3. **La inmutabilidad previene violaciones** — Las propiedades de solo lectura aseguran contratos claros
4. **La composición es más segura que la herencia** — Cuando el comportamiento difiere fundamentalmente, usa interfaces
5. **El código polimórfico debe funcionar sin verificación de tipos** — Si necesitas `if (shape is Square)`, has violado LSP

Esta es la forma correcta de modelar conceptos relacionados: **una abstracción común con implementaciones independientes que respetan contratos compartidos**.
