using SolidPrinciples.SRP.Problem;

namespace SolidPrinciples.SRP.Tests.Problem;

/// <summary>
/// PROBLEMA: Estos tests muestran por qué CommunityReportService es difícil de probar.
///
/// Nota:
/// - No podemos probar el formateo HTML en aislamiento — está enterrado en GenerateAndSendReport
/// - No podemos verificar que el correo fue enviado con el contenido correcto sin un servidor SMTP
/// - Cambiar el formato HTML requiere re-probar todo el pipeline
/// </summary>
public sealed class CommunityReportServiceTests
{
  [Fact]
  public void Constructor_RequiresInfrastructureParameters()
  {
    // Incluso la instanciación requiere conocimiento de infraestructura
    var sut = new CommunityReportService(
        connectionString: "Server=localhost;Database=Test;",
        smtpHost: "smtp.test.com");

    Assert.NotNull(sut);
  }

  // NOTA: ¿Quieres probar SOLO la lógica de formateo HTML?
  // No puedes — es una preocupación privada dentro de GenerateAndSendReport.
  //
  // ¿Quieres probar SOLO el formateo CSV?
  // No puedes — está embebido en GenerateAndSendCsvReport.
  //
  // ¿Quieres verificar que el correo fue enviado con el asunto correcto?
  // No puedes — SendEmail es un método privado con una dependencia SMTP real.
  //
  // La ÚNICA manera de probar esta clase es un test de integración con infraestructura real.
}
