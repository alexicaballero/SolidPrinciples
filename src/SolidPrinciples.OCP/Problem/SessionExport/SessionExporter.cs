using System.Text;
using System.Text.Json;

namespace SolidPrinciples.OCP.Problem;

/// <summary>
/// PROBLEMA: Esta clase utiliza una instrucción switch para decidir el formato de exportación.
/// Cada vez que se necesita un nuevo formato (PDF, Markdown, YAML...) DEBES modificar esta clase.
///
/// La clase NO está cerrada para modificación — debe ser editada para extender el comportamiento.
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - Un switch/case en una cadena de "formato" que crece con cada nuevo requisito
/// - El caso default lanza una excepción — una señal de un punto de extensión no planificado
/// - Todos los formatos viven en UNA clase — un cambio en serialización JSON riesga romper CSV
/// - Agregar PDF significa modificar y re-probar TODOS los formatos existentes
///
/// Qué sucede cuando se solicita un nuevo formato:
/// 1. Abre este archivo
/// 2. Agrega un nuevo case al switch
/// 3. Escribe la lógica de formateo en línea
/// 4. Re-prueba TODOS los casos existentes (comparten el mismo método)
/// 5. Riesgo de romper algo que ya funcionaba
/// </remarks>
public sealed class SessionExporter
{
  /// <summary>
  /// Exporta una sesión al formato especificado.
  /// Cada nuevo formato requiere modificar este método.
  /// </summary>
  public string Export(SessionExportData session, string format)
  {
    // VIOLACIÓN: Instrucción switch que debe ser modificada para cada nuevo formato
    switch (format.ToLowerInvariant())
    {
      case "json":
        return JsonSerializer.Serialize(session, new JsonSerializerOptions
        {
          WriteIndented = true
        });

      case "csv":
        return $"{session.Id},{session.Title},{session.Speaker},{session.ScheduledAt:O},{session.Status}";

      case "xml":
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

      // VIOLACIÓN: ¿Y si alguien solicita PDF? ¿Markdown? ¿YAML?
      // Debes venir AQUÍ y agregar más cases. Esta clase NUNCA termina.
      default:
        throw new NotSupportedException($"El formato de exportación '{format}' no es compatible.");
    }
  }

  /// <summary>
  /// Exporta múltiples sesiones — mismo problema switch, ahora duplicado.
  /// </summary>
  public string ExportAll(IReadOnlyList<SessionExportData> sessions, string format)
  {
    // VIOLACIÓN: La misma lógica de selección de formato se duplica para exportación por lotes
    switch (format.ToLowerInvariant())
    {
      case "json":
        return JsonSerializer.Serialize(sessions, new JsonSerializerOptions
        {
          WriteIndented = true
        });

      case "csv":
        var csv = new StringBuilder();
        csv.AppendLine("Id,Title,Speaker,ScheduledAt,Status");
        foreach (var s in sessions)
          csv.AppendLine($"{s.Id},{s.Title},{s.Speaker},{s.ScheduledAt:O},{s.Status}");
        return csv.ToString();

      case "xml":
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

      default:
        throw new NotSupportedException($"El formato de exportación '{format}' no es compatible.");
    }
  }
}
