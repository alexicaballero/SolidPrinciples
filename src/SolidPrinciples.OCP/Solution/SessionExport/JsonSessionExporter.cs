using System.Text.Json;

namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// CORRECTO: Estrategia de exportación JSON — una unidad independiente y testeable.
/// Agregar este formato requirió CERO cambios en el código existente.
/// </summary>
public sealed class JsonSessionExporter : ISessionExportStrategy
{
    /// <inheritdoc />
    public string Format => "json";

    /// <inheritdoc />
    public string Export(SessionExportData session) =>
        JsonSerializer.Serialize(session, new JsonSerializerOptions { WriteIndented = true });

    /// <inheritdoc />
    public string ExportAll(IReadOnlyList<SessionExportData> sessions) =>
        JsonSerializer.Serialize(sessions, new JsonSerializerOptions { WriteIndented = true });
}
