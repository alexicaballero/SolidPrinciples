namespace SolidPrinciples.OCP.Problem;

/// <summary>
/// Simple DTO que representa datos de sesión para exportación.
/// Compartido entre ejemplos de Problema y Solución.
/// </summary>
public sealed record SessionExportData(
    Guid Id,
    string Title,
    string Speaker,
    DateTimeOffset ScheduledAt,
    string Status);
