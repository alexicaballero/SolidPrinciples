using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// SOLUCIÓN: Abstracción para persistencia de sesión.
/// El código de alto nivel depende de esta interfaz, no de implementaciones concretas.
/// </summary>
/// <remarks>
/// Esta interfaz define el contrato para persistencia de sesión.
/// Pueden existir múltiples implementaciones (SQL, MongoDB, InMemory), y el código
/// de alto nivel no necesita cambiar al cambiar entre ellas.
/// </remarks>
public interface ISessionRepository
{
    Task SaveAsync(Session session, CancellationToken cancellationToken);
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
