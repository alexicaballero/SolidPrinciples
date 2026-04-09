using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEMA: Implementación concreta de repositorio EF Core.
/// CommunityService de alto nivel depende de esto directamente.
/// </summary>
/// <remarks>
/// Esta clase es infraestructura de bajo nivel. Debería estar detrás de una abstracción.
/// CommunityService NO debería importar EFCoreCommunityRepository directamente.
/// </remarks>
public sealed class EFCoreCommunityRepository
{
    /// <summary>
    /// Agrega una comunidad a la base de datos usando EF Core.
    /// CommunityService depende directamente de este método específico de EF Core.
    /// </summary>
    public async Task AddAsync(Community community, CancellationToken cancellationToken)
    {
        // Simulated EF Core DbContext.Add() + SaveChangesAsync()
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"[EF Core] Added community: {community.Name}");
    }

    /// <summary>
    /// Recupera una comunidad por ID usando EF Core.
    /// </summary>
    public async Task<Community?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Simulated EF Core DbContext.Set<Community>().FindAsync(id)
        await Task.Delay(10, cancellationToken);
        return null; // Simplified
    }
}
