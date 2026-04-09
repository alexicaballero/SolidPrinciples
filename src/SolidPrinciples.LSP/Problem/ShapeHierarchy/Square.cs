namespace SolidPrinciples.LSP.Problem.ShapeHierarchy;

/// <summary>
/// VIOLACIÓN: Square fuerza Width y Height a ser iguales, rompiendo el contrato de Rectangle.
/// </summary>
/// <remarks>
/// Esto viola LSP porque:
/// 1. Rectangle promete que Width y Height son INDEPENDIENTES
/// 2. Square las acopla (modificar una cambia la otra)
/// 3. El código que espera propiedades independientes obtiene EFECTOS SECUNDARIOS OCULTOS
///
/// Tipo de violación: POSTCONDICIONES OCULTAS
/// - Establecer Width cambia Height (no documentado en la clase base)
/// - Establecer Height cambia Width (comportamiento inesperado)
///
/// Por qué esto es una violación:
/// - Código cliente: rect.Width = 5; rect.Height = 10;
/// - Con Rectangle: Width=5, Height=10 ✓ Esperado
/// - Con Square: Width=10, Height=10 ✗ Roto (el último setter gana)
///
/// La relación matemática "es-un" (un cuadrado ES-UN rectángulo)
/// ¡NO se traduce a herencia en código!
/// </remarks>
public class Square : Rectangle
{
    /// <summary>
    /// VIOLACIÓN: Establecer Width también establece Height (efecto secundario oculto).
    /// </summary>
    public override int Width
    {
        get => base.Width;
        set
        {
            base.Width = value;
            base.Height = value; // ¡Efecto secundario! Rompe el contrato de independencia
        }
    }

    /// <summary>
    /// VIOLACIÓN: Establecer Height también establece Width (efecto secundario oculto).
    /// </summary>
    public override int Height
    {
        get => base.Height;
        set
        {
            base.Height = value;
            base.Width = value; // ¡Efecto secundario! Rompe el contrato de independencia
        }
    }
}
