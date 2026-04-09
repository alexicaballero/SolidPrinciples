using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Repositorio en memoria para pruebas.
/// Implementa la misma abstracción que SqlSessionRepository.
/// Se puede inyectar en CreateSessionHandler para pruebas unitarias.
/// </summary>
/// <remarks>
/// Este es el poder de DIP: Puedes intercambiar implementaciones sin cambiar
/// código de alto nivel. En pruebas, usa InMemorySessionRepository. En producción,
/// usa SqlSessionRepository.
/// </remarks>
public sealed class InMemorySessionRepository : ISessionRepository
{
    private readonly List<Session> _sessions = [];

    public Task SaveAsync(Session session, CancellationToken cancellationToken)
    {
        _sessions.Add(session);
        return Task.CompletedTask;
    }

    public Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(session);
    }
}
