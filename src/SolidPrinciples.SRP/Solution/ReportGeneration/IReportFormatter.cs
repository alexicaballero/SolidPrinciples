namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Interfaz de formateador — responsable SÓLO de convertir datos a un formato específíco.
/// Agregar un nuevo formato (PDF, Markdown) significa crear una nueva clase, no modificar las existentes.
/// </summary>
public interface IReportFormatter
{
  string Format(IReadOnlyList<SessionReportItem> sessions);
}
