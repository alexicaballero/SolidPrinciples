using FluentAssertions;
using SolidPrinciples.OCP.Solution;

namespace SolidPrinciples.OCP.Tests.Solution;

/// <summary>
/// Tests para el SessionExportService (el orquestador).
/// 
/// BENEFICIO CLAVE: El servicio se prueba con estrategias inyectadas —
/// agregar un nuevo formato requiere CERO cambios en estos tests.
/// </summary>
public sealed class SessionExportServiceTests
{
  private static SessionExportData CreateSampleSession() =>
      new(
          Id: Guid.Parse("aaaa1111-bbbb-cccc-dddd-eeee2222ffff"),
          Title: "Introduction to SOLID",
          Speaker: "Jane Doe",
          ScheduledAt: new DateTimeOffset(2025, 6, 15, 10, 0, 0, TimeSpan.Zero),
          Status: "Scheduled");

  private static SessionExportService CreateServiceWithAllStrategies() =>
      new([new JsonSessionExporter(), new CsvSessionExporter(), new XmlSessionExporter()]);

  [Theory]
  [InlineData("json")]
  [InlineData("csv")]
  [InlineData("xml")]
  public void Export_RegisteredFormat_ReturnsNonEmptyResult(string format)
  {
    var sut = CreateServiceWithAllStrategies();

    var result = sut.Export(CreateSampleSession(), format);

    result.Should().NotBeNullOrWhiteSpace();
    result.Should().Contain("Introduction to SOLID");
  }

  [Fact]
  public void Export_UnregisteredFormat_ThrowsNotSupportedException()
  {
    var sut = CreateServiceWithAllStrategies();

    var act = () => sut.Export(CreateSampleSession(), "pdf");

    act.Should().Throw<NotSupportedException>()
        .WithMessage("*pdf*");
  }

  [Fact]
  public void AvailableFormats_ReturnsAllRegistered()
  {
    var sut = CreateServiceWithAllStrategies();

    sut.AvailableFormats.Should().Contain("json");
    sut.AvailableFormats.Should().Contain("csv");
    sut.AvailableFormats.Should().Contain("xml");
  }

  /// <summary>
  /// Demuestra extensibilidad: agregar una nueva estrategia no requiere cambios en el servicio.
  /// </summary>
  [Fact]
  public void Export_NewStrategyAddedAtRuntime_WorksWithoutModification()
  {
    // Simular agregar un exportador "markdown" en una fecha posterior
    var markdownExporter = new MarkdownSessionExporterStub();
    var sut = new SessionExportService([new JsonSessionExporter(), markdownExporter]);

    var result = sut.Export(CreateSampleSession(), "markdown");

    result.Should().Contain("# Introduction to SOLID");
  }

  /// <summary>
  /// Estrategia stub para probar extensibilidad — esto podría vivir en un ensamblado de plugin.
  /// </summary>
  private sealed class MarkdownSessionExporterStub : ISessionExportStrategy
  {
    public string Format => "markdown";

    public string Export(SessionExportData session) =>
        $"# {session.Title}\n\n**Speaker:** {session.Speaker}";

    public string ExportAll(IReadOnlyList<SessionExportData> sessions) =>
        string.Join("\n\n---\n\n", sessions.Select(Export));
  }
}
