namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// SOLUCIÓN: Fábrica de figuras que crea las figuras apropiadas.
/// </summary>
/// <remarks>
/// El patrón factory ayuda a los clientes a crear figuras sin preocuparse
/// por qué tipo concreto usar.
/// </remarks>
public static class ShapeFactory
{
    /// <summary>
    /// Crea un rectángulo con dimensiones independientes.
    /// </summary>
    public static IShape CreateRectangle(int width, int height)
    {
        return new Rectangle(width, height);
    }

    /// <summary>
    /// Crea un cuadrado con lados uniformes.
    /// </summary>
    public static IShape CreateSquare(int side)
    {
        return new Square(side);
    }

    /// <summary>
    /// Crea la figura apropiada según las dimensiones.
    /// </summary>
    public static IShape CreateShape(int width, int height)
    {
        // Si las dimensiones son iguales, crear un cuadrado
        if (width == height)
            return new Square(width);

        // De lo contrario, crear un rectángulo
        return new Rectangle(width, height);
    }
}
