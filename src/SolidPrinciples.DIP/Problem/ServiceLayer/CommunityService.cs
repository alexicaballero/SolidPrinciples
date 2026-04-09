using SolidPrinciples.Common;

namespace SolidPrinciples.DIP.Problem;

/// <summary>
/// PROBLEMA: La capa de servicio depende directamente de clases concretas de infraestructura.
/// No se puede probar sin una base de datos real, no se pueden intercambiar implementaciones.
///
/// Esto viola DIP porque:
/// - CommunityService (política de alto nivel) depende de EFCoreRepository (detalle de bajo nivel)
/// - CommunityService importa paquetes de Entity Framework
/// - Agregar nueva implementación de repositorio requiere modificar CommunityService
/// </summary>
/// <remarks>
/// Señales de advertencia:
/// - El constructor del servicio llama a `new` en clases de infraestructura
/// - El servicio tiene referencias a controladores EF Core, Dapper, MongoDB
/// - Las pruebas requieren una conexión a base de datos
/// - Intercambiar la capa de persistencia requiere recompilar el servicio
/// </remarks>
public sealed class CommunityService
{
    private readonly EFCoreCommunityRepository _repository;
    private readonly FileSystemLogger _logger;

    /// <summary>
    /// VIOLATION: El constructor instancia dependencias concretas.
    /// El servicio está estrechamente acoplado a EF Core y registro del sistema de archivos.
    /// </summary>
    public CommunityService()
    {
        // VIOLATION: Service directly creates low-level dependencies
        _repository = new EFCoreCommunityRepository();
        _logger = new FileSystemLogger();
    }

    /// <summary>
    /// Crea una nueva comunidad.
    /// No se puede hacer pruebas unitarias sin una base de datos real y sistema de archivos.
    /// </summary>
    public async Task<Result> CreateCommunityAsync(string name, string description, CancellationToken cancellationToken)
    {
        _logger.Log($"Creating community: {name}");

        // VIOLACIÓN ADICIONAL: Asume que Create siempre tiene éxito sin manejar errores
        var community = Community.Create(name, description, Guid.NewGuid()).Value;

        // VIOLACIÓN: Llamar repositorio concreto
        await _repository.AddAsync(community, cancellationToken);

        _logger.Log($"Community created: {community.Id}");

        return Result.Success();
    }

    /// <summary>
    /// Obtiene una comunidad por ID.
    /// No se puede probar con datos en memoria.
    /// </summary>
    public async Task<Community?> GetCommunityAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.Log($"Fetching community: {id}");

        // VIOLATION: Calling concrete repository
        var community = await _repository.GetByIdAsync(id, cancellationToken);

        if (community != null)
        {
            _logger.Log($"Community found: {community.Name}");
        }

        return community;
    }
}
