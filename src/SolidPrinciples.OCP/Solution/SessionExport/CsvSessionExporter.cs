using System.Text;

namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// CORRECTO: Estrategia de exportación CSV — aislada de otros formatos.
/// Los cambios aquí no pueden romper la exportación JSON o XML.
/// </summary>
public sealed class CsvSessionExporter : ISessionExportStrategy
{
  /// <inheritdoc />
  public string Format => "csv";

  /// <inheritdoc />
  public string Export(SessionExportData session) =>
      $"{session.Id},{session.Title},{session.Speaker},{session.ScheduledAt:O},{session.Status}";

  /// <inheritdoc />
  public string ExportAll(IReadOnlyList<SessionExportData> sessions)
  {
    var csv = new StringBuilder();
    csv.AppendLine("Id,Título,Orador,ProgramadoEn,Estado");
    foreach (var s in sessions)
      csv.AppendLine($"{s.Id},{s.Title},{s.Speaker},{s.ScheduledAt:O},{s.Status}");
    return csv.ToString();
  }
}
