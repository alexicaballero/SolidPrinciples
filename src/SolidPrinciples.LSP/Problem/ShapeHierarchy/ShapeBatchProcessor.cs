namespace SolidPrinciples.LSP.Problem.ShapeHierarchy;

/// <summary>
/// PROBLEMA: Procesador por lotes que falla con colecciones mixtas de Rectangle/Square.
/// </summary>
public sealed class ShapeBatchProcessor
{
    /// <summary>
    /// Procesa un lote de rectángulos.
    /// Produce resultados INCORRECTOS cuando la colección contiene Squares.
    /// </summary>
    public IReadOnlyList<int> CalculateAreas(List<Rectangle> rectangles)
    {
        var areas = new List<int>();

        foreach (var rect in rectangles)
        {
            // Asumimos que podemos establecer dimensiones independientemente
            rect.Width = 4;
            rect.Height = 6;

            // Esperado: todas las áreas = 24
            // Con Square mezclado: algunas áreas = 36 (6*6)
            areas.Add(rect.CalculateArea());
        }

        return areas.AsReadOnly();
    }

    /// <summary>
    /// Intenta escalar todos los rectángulos por diferentes factores.
    /// FALLA con Square porque el escalado debe ser uniforme.
    /// </summary>
    public void ScaleShapes(List<Rectangle> rectangles, int widthFactor, int heightFactor)
    {
        foreach (var rect in rectangles)
        {
            rect.Width *= widthFactor;
            rect.Height *= heightFactor;

            // Con Rectangle: Width escala por widthFactor, Height por heightFactor ✓
            // Con Square: Ambos terminan escalados solo por heightFactor ✗ (el último escrito gana)
        }
    }
}
