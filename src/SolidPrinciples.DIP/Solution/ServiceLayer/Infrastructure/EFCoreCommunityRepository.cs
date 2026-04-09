using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Implementación concreta de repositorio EF Core.
/// Esta clase implementa la abstracción ICommunityRepository.
/// El código de alto nivel NO importa esta clase directamente.
/// </summary>
/// <remarks>
/// Registrado en contenedor DI:
/// services.AddScoped&lt;ICommunityRepository, EFCoreCommunityRepository&gt;();
///
/// Inversión de dependencia:
/// - CommunityService → ICommunityRepository (depende de abstracción)
/// - EFCoreCommunityRepository → ICommunityRepository (implementa abstracción)
/// </remarks>
public sealed class EFCoreCommunityRepository : ICommunityRepository
{
    public async Task AddAsync(Community community, CancellationToken cancellationToken)
    {
        // Lógica específica de EF Core: _context.Communities.Add(community); await _context.SaveChangesAsync();
        await Task.Delay(10, cancellationToken);
        Console.WriteLine($"[EF Core] Added community: {community.Name}");
    }

    public async Task<Community?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Lógica específica de EF Core: await _context.Communities.FindAsync(id);
        await Task.Delay(10, cancellationToken);
        return null;
    }
}
