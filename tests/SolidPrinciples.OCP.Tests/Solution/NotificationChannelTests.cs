using FluentAssertions;
using SolidPrinciples.OCP.Solution;

namespace SolidPrinciples.OCP.Tests.Solution;

/// <summary>
/// Tests para implementaciones individuales de canales de notificación.
/// 
/// BENEFICIO CLAVE: Cada canal se prueba de forma aislada.
/// Agregar un TeamsNotificationChannel significa escribir una nueva clase de test —
/// los tests existentes permanecen intactos.
/// </summary>
public sealed class NotificationChannelTests
{
  // --- Email ---

  [Fact]
  public void EmailChannel_Send_ReturnsFormattedEmail()
  {
    var sut = new EmailNotificationChannel();

    var result = sut.Send("user@mail.com", "Session created");

    result.Should().Contain("CORREO ELECTRÓNICO");
    result.Should().Contain("user@mail.com");
    result.Should().Contain("Session created");
  }

  [Fact]
  public void EmailChannel_Channel_IsEmail()
  {
    var sut = new EmailNotificationChannel();
    sut.Channel.Should().Be("email");
  }

  // --- SMS ---

  [Fact]
  public void SmsChannel_Send_TruncatesLongMessages()
  {
    var sut = new SmsNotificationChannel();
    var longMessage = new string('x', 200);

    var result = sut.Send("+1234567890", longMessage);

    result.Should().Contain("SMS");
    result.Should().NotContain(longMessage);
  }

  [Fact]
  public void SmsChannel_Send_KeepsShortMessages()
  {
    var sut = new SmsNotificationChannel();

    var result = sut.Send("+1234567890", "Short");

    result.Should().Contain("SMS");
    result.Should().Contain("Short");
  }

  // --- Slack ---

  [Fact]
  public void SlackChannel_Send_IncludesHashAndBold()
  {
    var sut = new SlackNotificationChannel();

    var result = sut.Send("general", "New session");

    result.Should().Contain("SLACK");
    result.Should().Contain("#general");
    result.Should().Contain("*New session*");
  }
}
