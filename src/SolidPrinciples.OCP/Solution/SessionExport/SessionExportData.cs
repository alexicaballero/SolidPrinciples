namespace SolidPrinciples.OCP.Solution;

/// <summary>
/// Simple DTO que representa datos de sesión para exportación.
/// </summary>
public sealed record SessionExportData(
    Guid Id,
    string Title,
    string Speaker,
    DateTimeOffset ScheduledAt,
    string Status);
