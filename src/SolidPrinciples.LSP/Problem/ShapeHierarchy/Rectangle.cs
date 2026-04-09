namespace SolidPrinciples.LSP.Problem.ShapeHierarchy;

/// <summary>
/// PROBLEMA: Clase base Rectangle con Width y Height independientes.
/// </summary>
/// <remarks>
/// Rectangle establece un contrato:
/// - Width y Height son propiedades independientes
/// - Modificar una NO afecta a la otra
/// - Área = Width * Height
///
/// Este contrato parece razonable para rectángulos.
/// </remarks>
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }

    public int CalculateArea()
    {
        return Width * Height;
    }
}
