using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// SOLUCIÓN: Abstracción para persistencia de comunidad.
/// El código de alto nivel depende de esta interfaz.
/// </summary>
/// <remarks>
/// Esta interfaz define el contrato para persistencia de comunidad.
/// Pueden existir múltiples implementaciones (EF Core, Dapper, MongoDB, InMemory).
/// </remarks>
public interface ICommunityRepository
{
    Task AddAsync(Community community, CancellationToken cancellationToken);
    Task<Community?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
