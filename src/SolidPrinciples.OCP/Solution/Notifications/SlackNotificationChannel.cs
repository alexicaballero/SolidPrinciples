namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// CORRECTO: Canal de notificación Slack — agregado SIN modificar ninguna clase existente.
/// </summary>
public sealed class SlackNotificationChannel : INotificationChannel
{
  /// <inheritdoc />
  public string Channel => "slack";

  /// <inheritdoc />
  public string Send(string recipient, string message) =>
      $"SLACK a #{recipient}: *{message}*";
}
