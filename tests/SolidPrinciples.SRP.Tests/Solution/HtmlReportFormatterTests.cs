using FluentAssertions;
using SolidPrinciples.SRP.Solution;

namespace SolidPrinciples.SRP.Tests.Solution;

/// <summary>
/// SOLUCIÓN: Tests del formateador — sin base de datos, sin correo electrónico, sin sistema de archivos.
/// Probamos SOLO la responsabilidad de formateo en completo aislamiento.
///
/// Comparar con Problem/CommunityReportServiceTests.cs donde no podíamos probar el formateo en absoluto.
/// </summary>
public sealed class HtmlReportFormatterTests
{
  [Fact]
  public void Format_WithSessions_ReturnsHtmlWithTableRows()
  {
    // Arrange — Solo datos, sin infraestructura
    var sessions = new List<SessionReportItem>
        {
            new("SOLID Workshop", "Jane Smith", DateTimeOffset.Now.AddDays(7), "Scheduled"),
            new("DDD Deep Dive", "John Doe", DateTimeOffset.Now.AddDays(14), "Scheduled")
        };

    var sut = new HtmlReportFormatter();

    // Act
    var html = sut.Format(sessions);

    // Assert
    html.Should().Contain("<html>");
    html.Should().Contain("<table>");
    html.Should().Contain("SOLID Workshop");
    html.Should().Contain("Jane Smith");
    html.Should().Contain("DDD Deep Dive");
  }

  [Fact]
  public void Format_WithEmptyList_ReturnsHtmlWithEmptyTable()
  {
    // Arrange
    var sessions = new List<SessionReportItem>();
    var sut = new HtmlReportFormatter();

    // Act
    var html = sut.Format(sessions);

    // Assert
    html.Should().Contain("<table>");
    html.Should().Contain("</table>");
    html.Should().NotContain("<td>");
  }
}
