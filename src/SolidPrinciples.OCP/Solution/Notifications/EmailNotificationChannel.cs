namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// CORRECTO: Canal de notificación por correo electrónico — autonónomo y testeable de forma independiente.
/// </summary>
public sealed class EmailNotificationChannel : INotificationChannel
{
  /// <inheritdoc />
  public string Channel => "email";

  /// <inheritdoc />
  public string Send(string recipient, string message) =>
      $"CORREO ELECTRÓNICO a {recipient}: {message}";
}
