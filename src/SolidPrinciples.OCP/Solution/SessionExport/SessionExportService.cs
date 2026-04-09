namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// SOLUCIÓN: Servicio que resuelve estrategias de exportación en tiempo de ejecución.
/// Está CERRADO para modificación — los nuevos formatos se conectan a través de DI,
/// y esta clase nunca cambia.
/// </summary>
/// <remarks>
/// Con inyección de dependencias, registrar una nueva implementación ISessionExportStrategy
/// (p. ej., PdfSessionExporter) la hace automáticamente disponible aquí.
/// Sin switch, sin if-else, sin modificación.
/// </remarks>
public sealed class SessionExportService(IEnumerable<ISessionExportStrategy> strategies)
{
    private readonly Dictionary<string, ISessionExportStrategy> _strategies =
        strategies.ToDictionary(s => s.Format, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Exporta una sesión utilizando el formato solicitado.
    /// Agregar un nuevo formato requiere CERO cambios en esta clase.
    /// </summary>
    public string Export(SessionExportData session, string format) =>
        GetStrategy(format).Export(session);

    /// <summary>
    /// Exporta múltiples sesiones utilizando el formato solicitado.
    /// </summary>
    public string ExportAll(IReadOnlyList<SessionExportData> sessions, string format) =>
        GetStrategy(format).ExportAll(sessions);

    /// <summary>
    /// Devuelve todos los nombres de formato disponibles.
    /// </summary>
    public IReadOnlyCollection<string> AvailableFormats => _strategies.Keys;

    private ISessionExportStrategy GetStrategy(string format) =>
        _strategies.TryGetValue(format, out var strategy)
            ? strategy
            : throw new NotSupportedException($"El formato de exportación '{format}' no es compatible. Disponibles: {string.Join(", ", _strategies.Keys)}");
}
