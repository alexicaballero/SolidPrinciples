namespace SolidPrinciples.ISP.Problem.GatheringRepositories;

/// <summary>
/// VIOLACIÓN ISP: Una interfaz de repositorio para TODO.
/// </summary>
/// <remarks>
/// Esta interfaz gorda combina:
/// - Operaciones de Session (7 métodos)
/// - Operaciones de Community (5 métodos)
/// - Operaciones de Resource (3 métodos)
/// - Persistencia (1 método)
/// = 16+ métodos en total
///
/// Problemas:
/// 1. Acoplamiento innecesario: CreateCommunityCommandHandler está acoplado a operaciones
///    de session, resource y community, pero solo necesita operaciones de community
/// 2. Difícil de probar: Hacer mock requiere implementar 16+ métodos aunque la prueba
///    solo ejercite 2
/// 3. Amplificación de cambios: Agregar un método de consulta de session fuerza
///    recompilación de cada clase que depende de IDataRepository
/// 4. SRP violado: La interfaz misma tiene múltiples responsabilidades
///
/// Del artículo: "CreateCommunityCommandHandler depende de 17+ métodos pero usa solo 2:
/// AddCommunity() y SaveChangesAsync()"
/// </remarks>
public interface IDataRepository
{
    // VIOLACIÓN: Operaciones de Session
    Task<Session?> GetSessionByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Session>> GetAllSessionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Session>> GetSessionsByCommunityIdAsync(
        Guid communityId, CancellationToken ct = default);
    Task<IReadOnlyList<Session>> GetActiveSessionsAsync(CancellationToken ct = default);
    void AddSession(Session session);
    void UpdateSession(Session session);
    void RemoveSession(Session session);

    // VIOLACIÓN: Operaciones de Community
    Task<Community?> GetCommunityByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Community>> GetAllCommunitiesAsync(CancellationToken ct = default);
    void AddCommunity(Community community);
    void UpdateCommunity(Community community);
    void RemoveCommunity(Community community);

    // VIOLACIÓN: Operaciones de Resource
    Task<IReadOnlyList<SessionResource>> GetResourcesBySessionIdAsync(
        Guid sessionId, CancellationToken ct = default);
    void AddResource(SessionResource resource);
    void RemoveResource(SessionResource resource);

    // VIOLACIÓN: Persistencia mezclada con consultas
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
