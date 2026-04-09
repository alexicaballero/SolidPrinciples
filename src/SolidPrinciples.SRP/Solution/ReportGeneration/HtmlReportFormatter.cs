using System.Text;

namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Formatea datos de sesión como una tabla HTML.
/// Esta clase tiene UNA razón para cambiar: si los requisitos del formato HTML cambian.
/// </summary>
public sealed class HtmlReportFormatter : IReportFormatter
{
  public string Format(IReadOnlyList<SessionReportItem> sessions)
  {
    var html = new StringBuilder();
    html.Append("<html><body>");
    html.Append("<h1>Informe de Comunidad</h1>");
    html.Append("<table><tr><th>Título</th><th>Orador</th><th>Fecha</th><th>Estado</th></tr>");

    foreach (var session in sessions)
    {
      html.Append($"<tr><td>{session.Title}</td><td>{session.Speaker}</td>");
      html.Append($"<td>{session.Date:yyyy-MM-dd}</td><td>{session.Status}</td></tr>");
    }

    html.Append("</table></body></html>");
    return html.ToString();
  }
}
