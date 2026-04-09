namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// SOLUCIÓN: Servicio de notificación que envía a través de canales registrados.
/// Está CERRADO para modificación — los nuevos canales se conectan a través de DI.
/// </summary>
/// <remarks>
/// ¿Quieres agregar Microsoft Teams? Crea un TeamsNotificationChannel : INotificationChannel
/// y regístralo en DI. Esta clase nunca cambia.
/// </remarks>
public sealed class NotificationService(IEnumerable<INotificationChannel> channels)
{
    private readonly Dictionary<string, INotificationChannel> _channels =
        channels.ToDictionary(c => c.Channel, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Envía una notificación a través del canal especificado.
    /// Agregar un nuevo canal requiere CERO cambios en esta clase.
    /// </summary>
    public string Send(string channel, string recipient, string message) =>
        GetChannel(channel).Send(recipient, message);

    /// <summary>
    /// Emite a múltiples canales.
    /// </summary>
    public List<string> Broadcast(string message, IReadOnlyList<(string Channel, string Recipient)> targets) =>
        targets.Select(t => Send(t.Channel, t.Recipient, message)).ToList();

    /// <summary>
    /// Devuelve todos los nombres de canal disponibles.
    /// </summary>
    public IReadOnlyCollection<string> AvailableChannels => _channels.Keys;

    private INotificationChannel GetChannel(string channel) =>
        _channels.TryGetValue(channel, out var ch)
            ? ch
            : throw new NotSupportedException($"El canal de notificación '{channel}' no es compatible. Disponibles: {string.Join(", ", _channels.Keys)}");
}
