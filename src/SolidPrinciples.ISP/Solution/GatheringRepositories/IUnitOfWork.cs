namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// SOLUCIÓN ISP - Paso 3: Preocupación de persistencia separada.
/// </summary>
/// <remarks>
/// IUnitOfWork es una interfaz de un solo método. Es el ejemplo definitivo de ISP:
/// una interfaz, una responsabilidad.
///
/// Características clave:
/// ✓ Segregado de los repositorios
/// ✓ Responsabilidad única: persistir cambios
/// ✓ Cualquier handler que necesite persistencia depende de esto, nada más
///
/// Del artículo (Gathering.Domain/Abstractions/IUnitOfWork.cs):
/// "IUnitOfWork es una interfaz de un solo método. Es el ejemplo definitivo de ISP:
/// una interfaz, una responsabilidad. Cualquier handler que necesite persistencia
/// depende de esto, nada más."
///
/// Patrón Unit of Work:
/// - Coordina transacciones entre múltiples repositorios
/// - Asegura atomicidad: todo se guarda o nada se guarda
/// - Separado de las operaciones de consulta/comando del repositorio
/// </remarks>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
