namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// CORRECTO: Square con una sola dimensión Side.
/// </summary>
/// <remarks>
/// Square tiene su PROPIO contrato apropiado:
/// - Solo una dimensión (Side) porque los lados son iguales por definición
/// - Sin acoplamiento oculto ni efectos secundarios
/// - No puede ser mal usado estableciendo dimensiones independientes
///
/// Insight clave: Square NO es un Rectangle en código, aunque
/// matemáticamente un cuadrado es un rectángulo especial.
/// En programación, "es-un" significa "es sustituible por", no "es un tipo de".
/// </remarks>
public sealed class Square : IShape
{
    public int Side { get; }

    public Square(int side)
    {
        if (side <= 0)
            throw new ArgumentException("Side must be positive", nameof(side));

        Side = side;
    }

    /// <summary>
    /// CORRECTO: Cálculo de área para cuadrado.
    /// </summary>
    public int CalculateArea() => Side * Side;
}
