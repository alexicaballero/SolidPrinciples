using FluentAssertions;
using SolidPrinciples.OCP.Solution;

namespace SolidPrinciples.OCP.Tests.Solution;

/// <summary>
/// Tests para estrategias individuales de exportación.
/// 
/// BENEFICIO CLAVE: Cada estrategia se prueba en AISLAMIENTO.
/// Agregar un nuevo formato (PdfSessionExporter) significa agregar una nueva clase de test —
/// los tests existentes nunca se tocan.
/// </summary>
public sealed class SessionExportStrategyTests
{
  private static SessionExportData CreateSampleSession() =>
      new(
          Id: Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff"),
          Title: "Introduction to SOLID",
          Speaker: "Jane Doe",
          ScheduledAt: new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero),
          Status: "Scheduled");

  // --- JSON Strategy ---

  [Fact]
  public void JsonExporter_Export_ReturnsValidJson()
  {
    var sut = new JsonSessionExporter();

    var result = sut.Export(CreateSampleSession());

    result.Should().Contain("\"Title\":");
    result.Should().Contain("Introduction to SOLID");
  }

  [Fact]
  public void JsonExporter_Format_IsJson()
  {
    var sut = new JsonSessionExporter();
    sut.Format.Should().Be("json");
  }

  // --- CSV Strategy ---

  [Fact]
  public void CsvExporter_Export_ReturnsCommaSeparated()
  {
    var sut = new CsvSessionExporter();

    var result = sut.Export(CreateSampleSession());

    result.Should().Contain("Introduction to SOLID");
    result.Should().Contain(",");
  }

  [Fact]
  public void CsvExporter_ExportAll_IncludesHeader()
  {
    var sut = new CsvSessionExporter();
    var sessions = new List<SessionExportData> { CreateSampleSession() };

    var result = sut.ExportAll(sessions);

    result.Should().StartWith("Id,Título,Orador,ProgramadoEn,Estado");
  }

  // --- XML Strategy ---

  [Fact]
  public void XmlExporter_Export_ReturnsXmlWithTags()
  {
    var sut = new XmlSessionExporter();

    var result = sut.Export(CreateSampleSession());

    result.Should().Contain("<title>Introduction to SOLID</title>");
    result.Should().Contain("<?xml version=\"1.0\"");
  }

  [Fact]
  public void XmlExporter_Format_IsXml()
  {
    var sut = new XmlSessionExporter();
    sut.Format.Should().Be("xml");
  }
}
