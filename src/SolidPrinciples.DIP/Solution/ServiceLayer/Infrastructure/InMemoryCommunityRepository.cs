using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// CORRECTO: Repositorio en memoria para pruebas.
/// Implementa ICommunityRepository, se puede inyectar en CommunityService para pruebas.
/// </summary>
/// <remarks>
/// Usado en pruebas unitarias:
/// var repository = new InMemoryCommunityRepository();
/// var logger = new ConsoleLogger();
/// var service = new CommunityService(repository, logger);
/// </remarks>
public sealed class InMemoryCommunityRepository : ICommunityRepository
{
    private readonly List<Community> _communities = [];

    public Task AddAsync(Community community, CancellationToken cancellationToken)
    {
        _communities.Add(community);
        return Task.CompletedTask;
    }

    public Task<Community?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var community = _communities.FirstOrDefault(c => c.Id == id);
        return Task.FromResult(community);
    }
}
