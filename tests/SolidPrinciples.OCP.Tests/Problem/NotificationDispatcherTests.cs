using FluentAssertions;
using SolidPrinciples.OCP.Problem;

namespace SolidPrinciples.OCP.Tests.Problem;

/// <summary>
/// Tests para el NotificationDispatcher del Problema.
/// 
/// PUNTOS DE DOLOR demostrados:
/// - Todos los canales probados a través de UNA clase con cadena if-else
/// - Agregar Teams o Push significa modificar código existente y re-ejecutar todos los tests
/// - Canales no soportados fallan en tiempo de ejecución, no en tiempo de compilación
/// </summary>
public sealed class NotificationDispatcherTests
{
  private readonly NotificationDispatcher _sut = new();

  [Fact]
  public void Send_Email_ReturnsEmailResult()
  {
    var result = _sut.Send("email", "user@mail.com", "Session created");

    result.Should().Contain("CORREO ELECTRÓNICO");
    result.Should().Contain("user@mail.com");
  }

  [Fact]
  public void Send_Sms_TruncatesLongMessages()
  {
    var longMessage = new string('x', 200);

    var result = _sut.Send("sms", "+1234567890", longMessage);

    result.Should().Contain("SMS");
    // El cuerpo del SMS debe truncarse a 160 caracteres
    result.Should().NotContain(longMessage);
  }

  [Fact]
  public void Send_Slack_ReturnsSlackFormattedResult()
  {
    var result = _sut.Send("slack", "general", "New session available");

    result.Should().Contain("SLACK");
    result.Should().Contain("#general");
  }

  /// <summary>
  /// Los nuevos canales lanzan excepción en tiempo de ejecución — la cadena if-else nunca está "completa".
  /// </summary>
  [Fact]
  public void Send_UnsupportedChannel_ThrowsNotSupportedException()
  {
    var act = () => _sut.Send("teams", "user@mail.com", "Hello");

    act.Should().Throw<NotSupportedException>()
        .WithMessage("*teams*");
  }

  [Fact]
  public void Broadcast_SendsToAllTargets()
  {
    var targets = new List<(string Channel, string Recipient)>
        {
            ("email", "user@mail.com"),
            ("sms", "+1234567890")
        };

    var results = _sut.Broadcast("Session created", targets);

    results.Should().HaveCount(2);
    results[0].Should().Contain("CORREO ELECTRÓNICO");
    results[1].Should().Contain("SMS");
  }
}
