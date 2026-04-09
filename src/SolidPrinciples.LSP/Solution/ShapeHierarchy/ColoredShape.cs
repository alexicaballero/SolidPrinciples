namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// SOLUCIÓN: Demuestra la composición de figuras con nuevos comportamientos.
/// </summary>
/// <remarks>
/// En lugar de usar herencia para agregar comportamiento, usamos composición.
/// Esto permite mezclar y combinar comportamientos sin violaciones de LSP.
/// </remarks>
public sealed class ColoredShape : IShape
{
    private readonly IShape _shape;
    private readonly string _color;

    public ColoredShape(IShape shape, string color)
    {
        _shape = shape ?? throw new ArgumentNullException(nameof(shape));
        _color = color ?? throw new ArgumentNullException(nameof(color));
    }

    public string Color => _color;

    /// <summary>
    /// CORRECTO: Delega el cálculo de área a la figura envuelta.
    /// </summary>
    public int CalculateArea() => _shape.CalculateArea();

    public string Describe() => $"{_color} shape with area {CalculateArea()}";
}
