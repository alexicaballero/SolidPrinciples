namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// CORRECTO: Rectangle con Width y Height independientes.
/// </summary>
/// <remarks>
/// Rectangle respeta su propio contrato:
/// - Width y Height son independientes (sin efectos secundarios)
/// - El constructor asegura un estado válido
/// - Las propiedades inmutables previenen inconsistencias
///
/// Nota: ¡No hay relación de herencia con Square!
/// </remarks>
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

    /// <summary>
    /// CORRECTO: Cálculo de área para rectángulo.
    /// </summary>
    public int CalculateArea() => Width * Height;
}
