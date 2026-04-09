using System.Text;

namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// CORRECTO: Estrategia de exportación XML — completamente independiente.
/// Agregar esto no requirió modificaciones en JsonSessionExporter ni CsvSessionExporter.
/// </summary>
public sealed class XmlSessionExporter : ISessionExportStrategy
{
  /// <inheritdoc />
  public string Format => "xml";

  /// <inheritdoc />
  public string Export(SessionExportData session)
  {
    var xml = new StringBuilder();
    xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    xml.Append("<session>");
    xml.Append($"<id>{session.Id}</id>");
    xml.Append($"<title>{session.Title}</title>");
    xml.Append($"<speaker>{session.Speaker}</speaker>");
    xml.Append($"<scheduledAt>{session.ScheduledAt:O}</scheduledAt>");
    xml.Append($"<status>{session.Status}</status>");
    xml.Append("</session>");
    return xml.ToString();
  }

  /// <inheritdoc />
  public string ExportAll(IReadOnlyList<SessionExportData> sessions)
  {
    var xml = new StringBuilder();
    xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    xml.Append("<sessions>");
    foreach (var s in sessions)
    {
      xml.Append("<session>");
      xml.Append($"<title>{s.Title}</title>");
      xml.Append($"<speaker>{s.Speaker}</speaker>");
      xml.Append("</session>");
    }
    xml.Append("</sessions>");
    return xml.ToString();
  }
}
