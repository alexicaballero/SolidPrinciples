namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Interfaz de proveedor de datos — responsable SÓLO de obtener datos del informe.
/// </summary>
public interface IReportDataProvider
{
  Task<IReadOnlyList<SessionReportItem>> GetSessionsByCommunityAsync(Guid communityId, CancellationToken ct = default);
}

/// <summary>
/// DTO de solo lectura para datos del informe.
/// </summary>
public sealed record SessionReportItem(
    string Title,
    string Speaker,
    DateTimeOffset Date,
    string Status);
