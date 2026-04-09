namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// SOLUCIÓN ISP - Paso 2: Interfaz de repositorio para Community.
/// </summary>
/// <remarks>
/// ICommunityRepository NO agrega métodos adicionales. La interfaz genérica
/// IRepository<Community> es suficiente para todas las operaciones de Community.
///
/// Patrón clave de ISP:
/// ✓ No agrega métodos innecesarios
/// ✓ La interfaz base es suficiente
/// ✓ Ningún cliente se ve forzado a depender de métodos que solo las sessions necesitan
///
/// Del artículo (Gathering.Domain/Communities/ICommunityRepository.cs):
/// "ICommunityRepository no agrega métodos. La interfaz genérica es suficiente.
/// Ningún cliente se ve forzado a depender de métodos que solo las sessions necesitan."
///
/// Nota: Esta interfaz existe como un tipo distinto para:
/// - Inyección de dependencias precisa
/// - Extensión futura si se necesitan consultas específicas de Community
/// - Claridad del dominio (ICommunityRepository vs IRepository<Community>)
/// </remarks>
public interface ICommunityRepository : IRepository<Community>
{
    // ¡No se necesitan métodos adicionales!
    // El IRepository<Community> base es suficiente
}
