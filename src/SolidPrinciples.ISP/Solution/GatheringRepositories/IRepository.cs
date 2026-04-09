using System.Linq.Expressions;

namespace SolidPrinciples.ISP.Solution.GatheringRepositories;

/// <summary>
/// SOLUCIÓN ISP - Paso 1: Repositorio base genérico.
/// </summary>
/// <remarks>
/// Esta es una interfaz enfocada y cohesiva. Cada método trata sobre operaciones
/// CRUD en un solo tipo de entidad. Cualquier repositorio para cualquier entidad
/// necesita estas operaciones.
///
/// Características clave:
/// ✓ Genérica: funciona con cualquier tipo Entity
/// ✓ Cohesiva: todos los métodos relacionados con CRUD
/// ✓ Completa: cubre las operaciones comunes necesarias
/// ✓ Sin persistencia: SaveChanges no está aquí (segregado en IUnitOfWork)
///
/// Del artículo (Gathering.Domain/Abstractions/IRepository.cs):
/// "Esta es una interfaz enfocada y cohesiva. Cada método trata sobre operaciones
/// CRUD en un solo tipo de entidad."
/// </remarks>
public interface IRepository<T> where T : Entity
{
    // Consultas
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Verificaciones de existencia
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Conteo
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // Comandos
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
