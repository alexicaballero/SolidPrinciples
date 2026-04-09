using FluentAssertions;
using SolidPrinciples.OCP.Problem;

namespace SolidPrinciples.OCP.Tests.Problem;

/// <summary>
/// Tests para el SessionExporter del Problema.
/// 
/// PUNTOS DE DOLOR demostrados:
/// - Todos los formatos se prueban a través de una ÚNICA clase
/// - No puedes agregar un nuevo formato sin modificar SessionExporter
/// - Probar un formato arriesga falsos negativos de otro
/// - Cada cambio de formato fuerza re-ejecutar TODOS los tests
/// </summary>
public sealed class SessionExporterTests
{
  private readonly SessionExporter _sut = new();

  private static SessionExportData CreateSampleSession() =>
      new(
          Id: Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff"),
          Title: "Introduction to SOLID",
          Speaker: "Jane Doe",
          ScheduledAt: new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero),
          Status: "Scheduled");

  [Fact]
  public void Export_Json_ReturnsJsonString()
  {
    var session = CreateSampleSession();

    var result = _sut.Export(session, "json");

    result.Should().Contain("\"Title\":");
    result.Should().Contain("Introduction to SOLID");
  }

  [Fact]
  public void Export_Csv_ReturnsCommaSeparatedValues()
  {
    var session = CreateSampleSession();

    var result = _sut.Export(session, "csv");

    result.Should().Contain("Introduction to SOLID");
    result.Should().Contain(",");
  }

  [Fact]
  public void Export_Xml_ReturnsXmlString()
  {
    var session = CreateSampleSession();

    var result = _sut.Export(session, "xml");

    result.Should().Contain("<title>Introduction to SOLID</title>");
  }

  /// <summary>
  /// Este test prueba que la clase NO está cerrada para modificación:
  /// los nuevos formatos explotan en tiempo de ejecución con NotSupportedException.
  /// </summary>
  [Fact]
  public void Export_UnsupportedFormat_ThrowsNotSupportedException()
  {
    var session = CreateSampleSession();

    var act = () => _sut.Export(session, "pdf");

    act.Should().Throw<NotSupportedException>()
        .WithMessage("*pdf*");
  }

  [Fact]
  public void ExportAll_Json_ReturnsJsonArray()
  {
    var sessions = new List<SessionExportData> { CreateSampleSession() };

    var result = _sut.ExportAll(sessions, "json");

    result.Should().Contain("[");
    result.Should().Contain("Introduction to SOLID");
  }
}
