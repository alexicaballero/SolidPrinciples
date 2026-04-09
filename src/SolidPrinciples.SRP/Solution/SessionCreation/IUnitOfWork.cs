namespace SolidPrinciples.SRP.Solution;

/// <summary>
/// SOLUCIÓN: Interfaz de unidad de trabajo — abstrae la gestión de transacciones como una preocupación separada.
/// </summary>
public interface IUnitOfWork
{
  Task SaveChangesAsync(CancellationToken ct = default);
}
