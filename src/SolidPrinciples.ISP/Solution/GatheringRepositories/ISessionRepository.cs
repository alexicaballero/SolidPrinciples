namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// SOLUCIÓN ISP - Paso 2: Interfaz de repositorio especializada para Session.
/// </summary>
/// <remarks>
/// ISessionRepository extiende IRepository<Session> con consultas específicas de Session
/// que van más allá del CRUD genérico.
///
/// Patrón clave de ISP:
/// ✓ Hereda operaciones CRUD genéricas de IRepository<Session>
/// ✓ Agrega solo consultas específicas de Session
/// ✓ Métodos como GetByCommunityIdAsync no tienen sentido para Communities
///
/// Del artículo (Gathering.Domain/Sessions/ISessionRepository.cs):
/// "ISessionRepository agrega solo consultas específicas de session. Métodos como
/// GetByCommunityIdAsync no tienen sentido para communities."
/// </remarks>
public interface ISessionRepository : IRepository<Session>
{
    // Solo consultas específicas de Session que van más allá del CRUD genérico
    Task<IReadOnlyList<Session>> GetByCommunityIdAsync(
        Guid communityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Session>> GetActiveSessionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SessionResource>> GetResourcesBySessionIdAsync(
        Guid sessionId,
        CancellationToken cancellationToken = default);

    void AddResource(SessionResource resource);
}
