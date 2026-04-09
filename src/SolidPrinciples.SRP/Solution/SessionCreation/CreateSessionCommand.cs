namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// Objeto de comando que representa la intención de crear una sesión.
/// </summary>
public sealed record CreateSessionCommand(
    Guid CommunityId,
    string Title,
    string Speaker,
    DateTimeOffset ScheduledAt);
