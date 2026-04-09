using SolidPrinciples.SRP.Problem;

namespace SolidPrinciples.SRP.Tests.Problem;

/// <summary>
/// PROBLEMA: Estos tests demuestran por qué las violaciones de SRP hacen que testear sea doloroso.
///
/// Nota:
/// - Necesitamos cadenas de conexión, hosts SMTP y rutas de archivos solo para instanciar
/// - No podemos probar la validación SIN también configurar la infraestructura de persistencia
/// - No podemos probar la lógica de negocio "crear" sin un servidor de correo electrónico
/// - Cada test es frágil porque depende de recursos externos
/// </summary>
public sealed class SessionManagerTests
{
  [Fact]
  public void CreateSession_WithEmptyTitle_ThrowsArgumentException()
  {
    // Arrange — Incluso para un test de validación simple, necesitamos TODOS los parámetros de infraestructura
    var sut = new SessionManager(
        connectionString: "Server=localhost;Database=Test;",
        smtpHost: "smtp.test.com",
        smtpPort: 587,
        logFilePath: "test.log");

    // Act & Assert — El test funciona, pero la configuración es innecesariamente pesada
    var act = () => sut.CreateSession("", "Speaker", DateTime.Now.AddDays(1), Guid.NewGuid());

    Assert.Throws<ArgumentException>(act);
  }

  [Fact]
  public void CreateSession_WithPastDate_ThrowsArgumentException()
  {
    // Arrange — La misma configuración pesada para otro test de validación simple
    var sut = new SessionManager(
        connectionString: "Server=localhost;Database=Test;",
        smtpHost: "smtp.test.com",
        smtpPort: 587,
        logFilePath: "test.log");

    // Act & Assert
    var act = () => sut.CreateSession("Title", "Speaker", DateTime.Now.AddDays(-1), Guid.NewGuid());

    Assert.Throws<ArgumentException>(act);
  }

  // NOTA: NO PODEMOS escribir un test para "CreateSession guarda exitosamente en la base de datos"
  // sin una base de datos real, porque el SQL está embebido directamente en la clase.
  //
  // NO PODEMOS probar "se envió la notificación" sin un servidor SMTP.
  //
  // NO PODEMOS probar "se escribió la entrada de registro" sin un sistema de archivos escribible.
  //
  // Este es el DOLOR de violar SRP — las responsabilidades están enredadas e imposibles de probar en aislamiento.
}
