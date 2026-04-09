using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Solution;

/// <summary>
/// SOLUCIÓN: La capa de servicio depende de abstracciones, no de infraestructura concreta.
/// Las dependencias se inyectan vía constructor.
///
/// Esto sigue DIP:
/// - CommunityService (alto nivel) depende de ICommunityRepository e ILogger (abstracciones)
/// - Las implementaciones concretas (EFCoreCommunityRepository, FileSystemLogger) dependen de las mismas abstracciones
/// - El servicio se puede probar con implementaciones en memoria/simuladas
/// </summary>
/// <remarks>
/// Beneficios:
/// - El servicio es testeable sin base de datos o sistema de archivos
/// - Fácil intercambiar implementaciones (EF Core → Dapper, Archivo → Azure Storage)
/// - El servicio se enfoca en lógica empresarial, no en infraestructura
/// - El contenedor DI gestiona la vida útil y el cableado
/// </remarks>
public sealed class CommunityService
{
    private readonly ICommunityRepository _repository;
    private readonly ILogger _logger;

    /// <summary>
    /// CORRECTO: Dependencias inyectadas vía constructor.
    /// Sintaxis de constructor primario para código limpio.
    /// </summary>
    public CommunityService(ICommunityRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Crea una nueva comunidad.
    /// Se puede hacer pruebas unitarias con repositorio en memoria y registrador de consola.
    /// </summary>
    public async Task<Result> CreateCommunityAsync(string name, string description, CancellationToken cancellationToken)
    {
        _logger.Log($"Creating community: {name}");

        // CORRECTO: Usar factory method de la entidad de dominio
        var communityResult = Community.Create(name, description, Guid.NewGuid());
        if (communityResult.IsFailure)
            return Result.Failure(communityResult.Error);

        // CORRECTO: Llamar métodos en abstracciones
        await _repository.AddAsync(communityResult.Value, cancellationToken);

        _logger.Log($"Community created: {communityResult.Value.Id}");

        return Result.Success();
    }

    /// <summary>
    /// Obtiene una comunidad por ID.
    /// Testeable con datos en memoria.
    /// </summary>
    public async Task<Community?> GetCommunityAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.Log($"Fetching community: {id}");

        // CORRECT: Calling methods on abstractions
        var community = await _repository.GetByIdAsync(id, cancellationToken);

        if (community != null)
        {
            _logger.Log($"Community found: {community.Name}");
        }

        return community;
    }
}
