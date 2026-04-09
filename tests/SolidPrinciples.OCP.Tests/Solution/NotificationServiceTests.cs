using FluentAssertions;
using SolidPrinciples.OCP.Solution;

namespace SolidPrinciples.OCP.Tests.Solution;

/// <summary>
/// Tests para el orquestador NotificationService.
/// 
/// BENEFICIO CLAVE: El servicio se prueba con canales inyectados —
/// agregar un nuevo canal requiere CERO cambios en estos tests.
/// </summary>
public sealed class NotificationServiceTests
{
  private static NotificationService CreateServiceWithAllChannels() =>
      new([new EmailNotificationChannel(), new SmsNotificationChannel(), new SlackNotificationChannel()]);

  [Theory]
  [InlineData("email", "user@mail.com")]
  [InlineData("sms", "+1234567890")]
  [InlineData("slack", "general")]
  public void Send_RegisteredChannel_ReturnsResult(string channel, string recipient)
  {
    var sut = CreateServiceWithAllChannels();

    var result = sut.Send(channel, recipient, "Test message");

    result.Should().NotBeNullOrWhiteSpace();
  }

  [Fact]
  public void Send_UnregisteredChannel_ThrowsNotSupportedException()
  {
    var sut = CreateServiceWithAllChannels();

    var act = () => sut.Send("teams", "user@mail.com", "Hello");

    act.Should().Throw<NotSupportedException>()
        .WithMessage("*teams*");
  }

  [Fact]
  public void Broadcast_SendsToAllTargets()
  {
    var sut = CreateServiceWithAllChannels();
    var targets = new List<(string Channel, string Recipient)>
        {
            ("email", "user@mail.com"),
            ("slack", "general")
        };

    var results = sut.Broadcast("New session", targets);

    results.Should().HaveCount(2);
    results[0].Should().Contain("CORREO ELECTRÓNICO");
    results[1].Should().Contain("SLACK");
  }

  [Fact]
  public void AvailableChannels_ReturnsAllRegistered()
  {
    var sut = CreateServiceWithAllChannels();

    sut.AvailableChannels.Should().Contain("email");
    sut.AvailableChannels.Should().Contain("sms");
    sut.AvailableChannels.Should().Contain("slack");
  }

  /// <summary>
  /// Demuestra extensibilidad: agregar un nuevo canal en tiempo de ejecución funciona sin modificación.
  /// </summary>
  [Fact]
  public void Send_NewChannelAddedAtRuntime_WorksWithoutModification()
  {
    var teamsChannel = new TeamsNotificationChannelStub();
    var sut = new NotificationService([new EmailNotificationChannel(), teamsChannel]);

    var result = sut.Send("teams", "user@mail.com", "Hello from Teams");

    result.Should().Contain("TEAMS");
  }

  private sealed class TeamsNotificationChannelStub : INotificationChannel
  {
    public string Channel => "teams";
    public string Send(string recipient, string message) =>
        $"TEAMS to {recipient}: {message}";
  }
}
