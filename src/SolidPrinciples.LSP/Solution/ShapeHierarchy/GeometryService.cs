namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// SOLUCIÓN: Servicio de geometría que trabaja con IShape polimórficamente.
/// </summary>
/// <remarks>
/// Este servicio demuestra cumplimiento de LSP:
/// - No se necesita verificación de tipos
/// - Sin casos especiales para diferentes figuras
/// - Todas las implementaciones de IShape funcionan correctamente
/// - Sin efectos secundarios ocultos ni expectativas rotas
/// </remarks>
public sealed class GeometryService
{
    /// <summary>
    /// CORRECTO: Calcula el área de cualquier figura sin importar el tipo.
    /// </summary>
    public int CalculateArea(IShape shape)
    {
        // Funciona con Rectangle, Square o cualquier IShape futuro
        // Sin verificación de tipos, sin casos especiales
        return shape.CalculateArea();
    }

    /// <summary>
    /// CORRECTO: Procesa múltiples figuras uniformemente.
    /// </summary>
    public IReadOnlyList<int> CalculateAreas(IEnumerable<IShape> shapes)
    {
        return shapes
            .Select(shape => shape.CalculateArea())
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Calcula el área total de todas las figuras.
    /// </summary>
    public int CalculateTotalArea(IEnumerable<IShape> shapes)
    {
        return shapes.Sum(shape => shape.CalculateArea());
    }

    /// <summary>
    /// Encuentra la figura más grande por área.
    /// </summary>
    public IShape? FindLargest(IEnumerable<IShape> shapes)
    {
        return shapes
            .OrderByDescending(shape => shape.CalculateArea())
            .FirstOrDefault();
    }
}
