namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// SOLUCIÓN: Abstracción que define una estrategia de exportación de sesión.
/// Los nuevos formatos se agregan creando nuevas implementaciones, NO modificando código existente.
/// </summary>
public interface ISessionExportStrategy
{
  /// <summary>
  /// El identificador de formato que esta estrategia soporta (p. ej., "json", "csv", "xml").
  /// </summary>
  string Format { get; }

  /// <summary>
  /// Exporta una sola sesión al formato de la estrategia.
  /// </summary>
  string Export(SessionExportData session);

  /// <summary>
  /// Exporta múltiples sesiones al formato de la estrategia.
  /// </summary>
  string ExportAll(IReadOnlyList<SessionExportData> sessions);
}
