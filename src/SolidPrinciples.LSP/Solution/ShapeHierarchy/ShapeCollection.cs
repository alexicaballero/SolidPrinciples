namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// SOLUCIÓN: Colección de figuras que funciona con cualquier IShape.
/// </summary>
public sealed class ShapeCollection
{
    private readonly List<IShape> _shapes = [];

    /// <summary>
    /// CORRECTO: Funciona con CUALQUIER IShape - Rectangle, Square, ColoredShape, etc.
    /// </summary>
    public void Add(IShape shape)
    {
        _shapes.Add(shape);
    }

    public IReadOnlyList<IShape> Shapes => _shapes.AsReadOnly();

    /// <summary>
    /// Obtiene figuras dentro de un rango de área determinado.
    /// </summary>
    public IReadOnlyList<IShape> GetShapesInAreaRange(int minArea, int maxArea)
    {
        return _shapes
            .Where(s => s.CalculateArea() >= minArea && s.CalculateArea() <= maxArea)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// CORRECTO: No se necesita verificación de tipos, solo polimorfismo.
    /// </summary>
    public int GetTotalArea()
    {
        return _shapes.Sum(shape => shape.CalculateArea());
    }
}
