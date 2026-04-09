namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// CORRECTO: Canal de notificación SMS — aislado de otros canales.
/// </summary>
public sealed class SmsNotificationChannel : INotificationChannel
{
  /// <inheritdoc />
  public string Channel => "sms";

  /// <inheritdoc />
  public string Send(string recipient, string message)
  {
    var truncated = message.Length > 160 ? message[..160] : message;
    return $"SMS a {recipient}: {truncated}";
  }
}
