namespace SolidPrinciples.LSP.Problem.ShapeHierarchy;

/// <summary>
/// PROBLEMA: Código cliente que confía en el contrato de Rectangle.
/// </summary>
/// <remarks>
/// Este servicio demuestra cómo las violaciones de LSP rompen el código cliente.
/// </remarks>
public sealed class GeometryService
{
    /// <summary>
    /// Redimensiona un rectángulo y verifica el área esperada.
    /// FALLA con Square debido a propiedades acopladas.
    /// </summary>
    public void ResizeAndVerify(Rectangle rect)
    {
        // El código cliente espera que Width y Height sean independientes
        rect.Width = 5;
        rect.Height = 10;

        // Esperado: 5 * 10 = 50
        int expectedArea = 50;
        int actualArea = rect.CalculateArea();

        // Con Rectangle: actualArea = 50 ✓
        // Con Square:    actualArea = 100 ✗ (¡Width fue sobrescrito a 10!)
        Console.WriteLine($"Expected: {expectedArea}, Actual: {actualArea}");
    }

    /// <summary>
    /// Verifica si un rectángulo es más ancho que alto.
    /// FALLA con Square porque las propiedades están acopladas.
    /// </summary>
    public bool IsLandscape(Rectangle rect)
    {
        rect.Width = 20;
        rect.Height = 10;

        // Esperado: true (20 > 10)
        // Con Rectangle: true ✓
        // Con Square: false ✗ (¡ambos son 10!)
        return rect.Width > rect.Height;
    }

    /// <summary>
    /// Demuestra el code smell de verificación de tipos.
    /// Cuando necesitas verificar tipos, la sustitución está rota.
    /// </summary>
    public int CalculateAreaSafely(Rectangle rect)
    {
        // OLOR DE CÓDIGO: Verificación de tipos necesaria porque Square rompe el contrato
        if (rect is Square square)
        {
            // Manejo especial para Square
            return square.Width * square.Width;
        }

        // Manejo normal para Rectangle
        rect.Width = 5;
        rect.Height = 10;
        return rect.CalculateArea();
    }
}
