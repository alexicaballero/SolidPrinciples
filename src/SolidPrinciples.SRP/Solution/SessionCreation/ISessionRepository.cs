namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Interfaz de repositorio — abstrae la persistencia como una responsabilidad separada.
/// El controlador depende de esta interfaz, no de SQL o ninguna base de datos concreta.
/// </summary>
public interface ISessionRepository
{
  Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default);
  void Add(Session session);
}
