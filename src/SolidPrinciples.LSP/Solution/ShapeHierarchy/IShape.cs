namespace SolidPrinciples.LSP.Solution.ShapeHierarchy;

/// <summary>
/// SOLUCIÓN: Abstracción común para figuras sin acoplamiento por herencia.
/// </summary>
/// <remarks>
/// IShape define SOLO el comportamiento común:
/// - Todas las figuras pueden calcular su área
/// - Sin suposiciones sobre cómo se almacenan las dimensiones
/// - Sin acoplamiento entre diferentes tipos de figuras
///
/// Patrón: Segregación de Interfaces + Composición sobre Herencia
/// </remarks>
public interface IShape
{
    int CalculateArea();
}
